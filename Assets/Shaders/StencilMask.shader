Shader "Custom/StencilMask" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

            ZWrite Off
            Lighting Off
            Fog { Mode Off }

         Zwrite off
        ColorMask 0
        Cull off

        Stencil {
            Ref 1
            Comp always
            Pass replace
        }
            Pass {
                Color[_Color]
            }
    }
}