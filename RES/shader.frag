#version 430
out vec4 FragColor;

layout(location = 0 ) uniform float iTime = 0;
layout(location = 1)  uniform vec3 iResolution;
layout(location = 2) uniform int iFrame;
layout(location = 3) uniform vec4 iMouse;
layout(location = 4) uniform float zoom;

layout(location = 5) uniform vec4[] colors;

layout(location = 9) uniform int flags;
//1 == single color
//2 == gradient
//4 == diff shading TODO


vec4 getCol(float coord,int ColNum)
{ 
    //Make these uniforms and allow user to select colors
    vec4[] cols =vec4[] (vec4(85,205,252,255),vec4(247,168,184,255),vec4(255),vec4(247,168,184,255),vec4(85,205,252,255));       
    int arrLength = 5;
    
    if(ColNum == 1) 
        return cols[0];
        
    float cstep1 = 1.0 / float(ColNum - 1);//Num of subgradients = num of colors - 1
    
    for(int i = 1; i < ColNum; i++)
    {
        if(coord < cstep1 * float(i))
        return mix(cols[int(mod(float(i-1),float(arrLength)))],cols[int(mod(float(i),float(arrLength)))], coord / cstep1 - float (i - 1));
    }    
    return vec4(coord);
}




vec4 GetColor(vec2 uv,float i,float maxI)
{
   if(i == maxI)
       return vec4(0);    
    //return (vec4(0,1,1,1) *(i /maxI)); 
   // if((flags & 1) == 1)
    return getCol((maxI * .15 + i)  / maxI,5) / 255;
}

void main( )
{    
    vec2 MinVals = vec2(-1.0,-0.5); 
    vec2 uv = (gl_FragCoord.xy / iResolution.y);//Get ss coords
    uv = (uv + MinVals)  / 0.42; //Offset and scale ss coords     
    vec2 Mouse = iMouse.xy / exp(zoom / 1000);
    uv /= zoom / 1000.0 * exp(zoom / 1000); 
    uv.x += Mouse.x / iResolution.x;
    uv.y -= Mouse.y / iResolution.y;


    vec2 coords = vec2(0);    
    vec2 coords2 = vec2(0);
    int iter = 0;
    int maxIter = min( iFrame / 10,1000);
   // maxIter = 5000;
    //Optimised escape time algorithm https://en.wikipedia.org/wiki/Plotting_algorithms_for_the_Mandelbrot_set
    while(dot(coords,coords)<= 4.0 && iter < maxIter)
    {
        coords.y = 2.0* coords.x * coords.y + uv.y;
        coords.x = coords2.x - coords2.y + uv.x;
        coords2 = coords * coords;
        iter++;
    }    
    // Output to screen
    FragColor = GetColor(uv,float(iter) , float(maxIter));
}