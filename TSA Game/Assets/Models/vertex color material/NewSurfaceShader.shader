Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Wireframe("Wireframe thickness", Range(0.0, 0.005)) = 0.0025
        _Transparency("Transparency", Range(0.0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
        }
    }
}
