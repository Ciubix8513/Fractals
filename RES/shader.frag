#version 430
out vec4 FragColor;

layout(location = 0 ) uniform float iTime = 0;
layout(location = 1)  uniform vec3 iResolution;
layout(location = 2) uniform int iFrame;
layout(location = 3) uniform vec4 iMouse;
layout(location = 4) uniform float zoom;

layout(location = 5) uniform vec4[] colors;


vec4 GetColor(vec2 uv,float i,float maxI)
{
   if(i == maxI)
       return vec4(0);    
    return (vec4(0,1,1,1) *(i /maxI)); 
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
    int maxIter = min( iFrame / 10,150);
    maxIter = 1000;
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