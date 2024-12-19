using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;
//Done so blend value is reset when going back into edit mode
[ExecuteInEditMode]
public class FullScreenEffectController : MonoBehaviour
{

//[SerializeField] private ScriptableRendererFeature _fullScreenDamage;
[SerializeField] private Material nauseaMat;

[SerializeField] private Material vigMat;

[SerializeField] private Material anxietyMat;
void Start(){

nauseaMat.SetFloat("_blend", 0);
vigMat.SetFloat("_Intensity",0);
anxietyMat.SetFloat("_Intensity",0);
}



public void setNauseaBlend(bool isSick, float sickAmount){
    float intensity = (float)math.remap(60,100,10,20,sickAmount);
    
    if(isSick){
 nauseaMat.SetFloat("_blend", 0.114f);
 nauseaMat.SetFloat("_Noise_Scale",intensity);
    }else{
nauseaMat.SetFloat("_blend", 0);

    }
}

public void setVigBlend(float sickAmount){
float vigBlend = (float)math.remap(0,100,.5,2,sickAmount);

vigMat.SetFloat("_Intensity",vigBlend);



}

public void setAnxietyIntensity(float cravingAmount){


    float anxietyIntensity = (float)math.remap(30,100,1,2,cravingAmount);
   
    if(anxietyIntensity >0){
    anxietyMat.SetFloat("_Intensity",anxietyIntensity);
    }
}





}
