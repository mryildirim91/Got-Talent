using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlatKitDepthNormals : ScriptableRendererFeature {
    class DepthNormalsPass : ScriptableRenderPass {
        private readonly int _depthBufferBits = 32;
        private RenderTargetHandle _depthAttachmentHandle;
        private RenderTextureDescriptor _descriptor;
        private readonly Material _depthNormalsMaterial = null;
        private FilteringSettings _filteringSettings;
        readonly string _profilerTag = "[Flat Kit] Depth Normals Pass";
        readonly ShaderTagId _shaderTagId = new ShaderTagId("DepthOnly");

        public DepthNormalsPass(RenderQueueRange renderQueueRange, LayerMask layerMask, Material material) {
            _filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            _depthNormalsMaterial = material;
        }

        public void Setup(RenderTextureDescriptor baseDescriptor, RenderTargetHandle depthAttachmentHandle) {
            this._depthAttachmentHandle = depthAttachmentHandle;
            baseDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            baseDescriptor.depthBufferBits = _depthBufferBits;
            _descriptor = baseDescriptor;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            cmd.GetTemporaryRT(_depthAttachmentHandle.id, _descriptor, FilterMode.Point);
            ConfigureTarget(_depthAttachmentHandle.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);

            using (new ProfilingScope(cmd, new ProfilingSampler(_profilerTag))) {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(_shaderTagId, ref renderingData, sortFlags);
                drawSettings.perObjectData = PerObjectData.None;

                ref CameraData cameraData = ref renderingData.cameraData;
                Camera camera = cameraData.camera;
#pragma warning disable 618
                if (cameraData.isStereoEnabled)
#pragma warning restore 618
                {
                    context.StartMultiEye(camera);
                }

                drawSettings.overrideMaterial = _depthNormalsMaterial;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings,
                    ref _filteringSettings);

                cmd.SetGlobalTexture("_CameraDepthNormalsTexture", _depthAttachmentHandle.id);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) {
            if (_depthAttachmentHandle == RenderTargetHandle.CameraTarget) return;
            cmd.ReleaseTemporaryRT(_depthAttachmentHandle.id);
            _depthAttachmentHandle = RenderTargetHandle.CameraTarget;
        }
    }

    DepthNormalsPass _depthNormalsPass;
    RenderTargetHandle _depthNormalsTexture;
    Material _depthNormalsMaterial;

    public FlatKitDepthNormals(RenderTargetHandle depthNormalsTexture) {
        _depthNormalsTexture = depthNormalsTexture;
    }

    public override void Create() {
        _depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
        _depthNormalsPass = new DepthNormalsPass(RenderQueueRange.opaque, -1, _depthNormalsMaterial) {
            renderPassEvent = RenderPassEvent.AfterRenderingPrePasses
        };
        _depthNormalsTexture.Init("_CameraDepthNormalsTexture");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        _depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor, _depthNormalsTexture);
        renderer.EnqueuePass(_depthNormalsPass);
    }
}