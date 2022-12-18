Shader "Custom/Mask"
{

  SubShader
  {
	 Tags {"Queue" = "Transparent+1"}	 
	 /*Cull off*/
  Pass
     {
		 Blend Zero One 
     }
  }

}
