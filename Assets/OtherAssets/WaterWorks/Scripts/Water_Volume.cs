using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material _material;

        private RTHandle tempColorRT;
        private RTHandle tempDepthRT;

        private RTHandle source;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
        }

        public void SetSource(RTHandle src)
        {
            source = src;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref tempColorRT, desc, name: "_TempColorRT");
            RenderingUtils.ReAllocateIfNeeded(ref tempDepthRT, desc, name: "_TempDepthRT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            if (_material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Water Volume Pass");

            // Первый Blit: копируем из source в временный RT с эффектом воды
            Blit(cmd, source, tempColorRT, _material);

            // Второй Blit: возвращаем результат в source
            Blit(cmd, tempColorRT, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            tempColorRT?.Release();
            tempDepthRT?.Release();
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();
    private CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
            settings.material = Resources.Load<Material>("Water_Volume");

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var cameraColorTarget = renderer.cameraColorTargetHandle;
        m_ScriptablePass.SetSource(cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
