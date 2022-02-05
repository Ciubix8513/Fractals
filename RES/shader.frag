#version 460
#pragma optionNV(fastmath off)
#pragma optionNV(fastprecision off)

out vec4 FragColor;

layout(location = 0 ) uniform float iTime = 0;
layout(location = 1)  uniform vec3 iResolution;
layout(location = 2) uniform int iFrame;
layout(location = 3) uniform vec4 iMouse;
layout(location = 4) uniform float zoom;

layout(location = 5) uniform int arrLength;
layout (std430,binding = 0) buffer colors
{
    vec4 cols[]; 
};



vec4 getCol(float coord,int ColNum)
{      
    if(ColNum == 1) 
        return cols[0];        
    float cstep1 = 1.0 / float(ColNum - 1); //Num of subgradients = num of colors - 1    
    for(int i = 1; i < ColNum; i++)    
        if(coord < cstep1 * float(i))
        return mix(cols[int(mod(float(i-1),float(arrLength)))],cols[int(mod(float(i),float(arrLength)))], coord / cstep1 - float (i - 1));        
    return vec4(coord);
}




vec4 GetColor(vec2 uv,float i,float maxI)
{
   if(i == maxI)
       return vec4(0);   
    return getCol((maxI * .15 + i)  / maxI,120) / 255;
}

vec4 fractal(vec2 C)
{

vec2 coords = vec2(0);    
    vec2 coords2 = vec2(0);
    int iter = 0;
    int maxIter = min( iFrame / 10,1000);
    maxIter = 1500;
    //Optimised escape time algorithm https://en.wikipedia.org/wiki/Plotting_algorithms_for_the_Mandelbrot_set
    while(dot(coords,coords)<= 4.0 && iter < maxIter)
    {
        coords.y = 2.0* coords.x * coords.y + C.y;
        coords.x = coords2.x - coords2.y + C.x;
        coords2 = coords * coords;
        iter++;
    }    
    return GetColor(C,float(iter) , float(maxIter));
}


void main( )
{    
   
    vec2 uv = gl_FragCoord.xy - (iResolution.xy *.5);
    vec2 c = vec2(uv * vec2(1.0,-1.0) /zoom- iMouse.xy );

    FragColor = fractal(c);
    
};