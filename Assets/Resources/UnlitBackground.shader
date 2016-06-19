Shader "Unlit/UnlitBackground"{
 
Properties {
    _Color ("Color", Color) = (1,1,1)
}
 
SubShader {
	Tags { "Queue"="Background" }
    Color [_Color]
    Pass {}
}
 
}


