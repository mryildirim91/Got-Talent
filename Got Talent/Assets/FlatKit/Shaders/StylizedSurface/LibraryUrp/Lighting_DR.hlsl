#ifndef FLATKIT_LIGHTING_DR_INCLUDED
#define FLATKIT_LIGHTING_DR_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

inline half NdotLTransition(half3 normal, half3 lightDir, half selfShadingSize, half shadowEdgeSize, half flatness) {
    half NdotL = dot(normal, lightDir);
    half angleDiff = saturate((NdotL * 0.5 + 0.5) - selfShadingSize);
    half angleDiffTransition = smoothstep(0, shadowEdgeSize, angleDiff); 
    return lerp(angleDiff, angleDiffTransition, flatness);
}

inline half NdotLTransitionPrimary(half3 normal, half3 lightDir) { 
    return NdotLTransition(normal, lightDir, _SelfShadingSize, _ShadowEdgeSize, _Flatness);
}

#if defined(DR_CEL_EXTRA_ON)
inline half NdotLTransitionExtra(half3 normal, half3 lightDir) { 
    return NdotLTransition(normal, lightDir, _SelfShadingSizeExtra, _ShadowEdgeSizeExtra, _FlatnessExtra);
}
#endif

inline half NdotLTransitionTexture(half3 normal, half3 lightDir, sampler2D stepTex) {
    half NdotL = dot(normal, lightDir);
    half angleDiff = saturate((NdotL * 0.5 + 0.5) - _SelfShadingSize * 0.0);
    half4 rampColor = tex2D(stepTex, half2(angleDiff, 0.5));
    // NOTE: The color channel here corresponds to the texture format in the shader editor script.
    half angleDiffTransition = rampColor.r;
    return angleDiffTransition;
}

half3 LightingPhysicallyBased_DSTRM(Light light, half3 normalWS, half3 viewDirectionWS, float3 positionWS)
{
    // If all light in the scene is baked, we use custom light direction for the cel shading.
    light.direction = lerp(light.direction, _LightmapDirection, _OverrideLightmapDir);

    half4 c = _BaseColor;

#if defined(_CELPRIMARYMODE_SINGLE)
    half NdotLTPrimary = NdotLTransitionPrimary(normalWS, light.direction);
    c = lerp(UNITY_ACCESS_INSTANCED_PROP(Props, _ColorDim), c, NdotLTPrimary);
#endif  // _CELPRIMARYMODE_SINGLE

#if defined(_CELPRIMARYMODE_STEPS)
    half NdotLTSteps = NdotLTransitionTexture(normalWS, light.direction, _CelStepTexture);
    c = lerp(_ColorDimSteps, c, NdotLTSteps);
#endif  // _CELPRIMARYMODE_STEPS

#if defined(_CELPRIMARYMODE_CURVE)
    half NdotLTCurve = NdotLTransitionTexture(normalWS, light.direction, _CelCurveTexture);
    c = lerp(_ColorDimCurve, c, NdotLTCurve);
#endif  // _CELPRIMARYMODE_CURVE

#if defined(DR_CEL_EXTRA_ON)
    half NdotLTExtra = NdotLTransitionExtra(normalWS, light.direction);
    c = lerp(_ColorDimExtra, c, NdotLTExtra);
#endif  // DR_CEL_EXTRA_ON

#if defined(DR_GRADIENT_ON)
    float angleRadians = _GradientAngle / 180.0 * PI;
    float posGradRotated = (positionWS.x - _GradientCenterX) * sin(angleRadians) + 
                           (positionWS.y - _GradientCenterY) * cos(angleRadians);
    float gradientTop = _GradientCenterY + _GradientSize * 0.5;
    half gradientFactor = saturate((gradientTop - posGradRotated) / _GradientSize);
    c = lerp(c, _ColorGradient, gradientFactor);
#endif  // DR_GRADIENT_ON

#if defined(DR_RIM_ON)
    half NdotL = dot(normalWS, light.direction);
    float4 rim = 1.0 - dot(viewDirectionWS, normalWS);
    float rimLightAlign = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimLightAlign);
    float rimSize = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimSize);
    float rimSpread = 1.0 - rimSize - NdotL * rimLightAlign;
    float rimEdgeSmooth = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimEdgeSmoothness);
    float rimTransition = smoothstep(rimSpread - rimEdgeSmooth * 0.5, rimSpread + rimEdgeSmooth * 0.5, rim);
    c = lerp(c, UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimColor), rimTransition);
#endif  // DR_RIM_ON

#if defined(DR_SPECULAR_ON)
    // Halfway between lighting direction and view vector.
    float3 halfVector = normalize(light.direction + viewDirectionWS);
    float NdotH = dot(normalWS, halfVector) * 0.5 + 0.5;
    float specularSize = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularSize);
    float specEdgeSmooth = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularEdgeSmoothness);
    float specular = saturate(pow(NdotH, 100.0 * (1.0 - specularSize) * (1.0 - specularSize)));
    float specularTransition = smoothstep(0.5 - specEdgeSmooth * 0.1, 0.5 + specEdgeSmooth * 0.1, specular);
    c = lerp(c, UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularColor), specularTransition);
#endif  // DR_SPECULAR_ON

#if defined(_UNITYSHADOWMODE_MULTIPLY)
    c *= lerp(1, light.shadowAttenuation, _UnityShadowPower);
#endif
#if defined(_UNITYSHADOWMODE_COLOR)
    half4 unityShadowColor = UNITY_ACCESS_INSTANCED_PROP(Props, _UnityShadowColor);
    c = lerp(lerp(c, unityShadowColor, unityShadowColor.a), c, light.shadowAttenuation);
#endif

    c.rgb *= light.color * light.distanceAttenuation;

    return c.rgb;
}

void StylizeLight(inout Light light)
{
    half shadowAttenuation = saturate(light.shadowAttenuation * _UnityShadowSharpness);
    light.shadowAttenuation = shadowAttenuation;

    half distanceAttenuation = smoothstep(0, _LightFalloffSize, light.distanceAttenuation);
    light.distanceAttenuation = distanceAttenuation;
        
    half3 lightColor = lerp(half3(1, 1, 1), light.color, _LightContribution);
    light.color = lightColor;
}

half4 UniversalFragment_DSTRM(InputData inputData, half3 albedo, half3 emission, half alpha)
{
    Light mainLight = GetMainLight(inputData.shadowCoord);
#if LIGHTMAP_ON
    mainLight.distanceAttenuation = 1.0;
#endif
    StylizeLight(mainLight);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

    // Apply Flat Kit stylizing to `inputData.bakedGI`.
#if LIGHTMAP_ON
    half sharpness01 = saturate((_UnityShadowSharpness - 1.0) / 9.0);  // UI range is set to 1.0 - 9.0.
    half shadowBlur = ((1.0 - sharpness01) * 0.5) * /*scale=*/0.5;
    inputData.bakedGI = smoothstep(0.5 - shadowBlur, 0.5 + shadowBlur, length(inputData.bakedGI));

    #if defined(_UNITYSHADOWMODE_MULTIPLY)
        inputData.bakedGI *= _UnityShadowPower;
    #endif
    #if defined(_UNITYSHADOWMODE_COLOR)
        half4 unityShadowColor = UNITY_ACCESS_INSTANCED_PROP(Props, _UnityShadowColor);
        inputData.bakedGI = lerp(inputData.bakedGI, unityShadowColor, unityShadowColor.a * inputData.bakedGI);
    #endif
#endif

    BRDFData brdfData;
    InitializeBRDFData(albedo, 1.0 - 1.0 / kDieletricSpec.a, 0, 0, alpha, brdfData);
    half3 color = GlobalIllumination(brdfData, inputData.bakedGI, 1.0, inputData.normalWS, inputData.viewDirectionWS);
    color += LightingPhysicallyBased_DSTRM(mainLight, inputData.normalWS, inputData.viewDirectionWS, inputData.positionWS);

#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
        StylizeLight(light);
        color += LightingPhysicallyBased_DSTRM(light, inputData.normalWS, inputData.viewDirectionWS, inputData.positionWS);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    color += inputData.vertexLighting * brdfData.diffuse;
#endif

    color += emission;
    return half4(color, alpha);
}

#endif // FLATKIT_LIGHTING_DR_INCLUDED