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
layout(location = 6)uniform int Time;
layout (location =7) uniform int cFractal;
layout(location = 8) uniform int maxIteration;
layout(location = 9) uniform int colorNum;
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
    return getCol((maxI * .15 + i)  / maxI,colorNum) / 255;
}

vec2 complexPow(vec2 z, float n)
{
    float r = length(z);
    float theta = atan( z.y , z.x);
    return pow(r,n) * vec2(cos(n * theta),sin(n*theta) ) ;
}

vec2 complexCube(vec2 z)
{
    return vec2(z.x*z.x*z.x -3 *z.x *z.y*z.y,3 * z.x*z.x*z.y - z.y*z.y*z.y);
}

vec2 complexDiv(vec2 a,vec2 b)
{
    return vec2((a.x *b.x + a.y * b.y )/(b.x*b.x + b.y*b.y),(a.y * b.x - a.x *b.y) / (b.x*b.x + b.y*b.y));
}

vec2 complexSquare(vec2 z)
{
    return vec2(z.x *z.x - z.y*z.y,2. * z.x * z.y);
}

vec2 complexSQRT(vec2 z)
{
    float l = length(z);
    return vec2(sqrt((l + z.x )/ 2),sign(z.y) * sqrt((l - z.x ) / 2));
}

vec2 mandelbrot(vec2 z, vec2 c)
{
    //Thanks IQ  https://iquilezles.org/www/articles/mset_1bulb/mset1bulb.htm
    float c2 = dot(c,c);
    if( 256.0*c2*c2 - 96.0*c2 + 32.0*c.x - 3.0 < 0.0 ) return vec2(6.9,420);
    if( 16.0*(c2+2.0*c.x+1.0) - 1.0 < 0.0 ) return vec2(6.9,420);
    return complexSquare(z)+ c; //z is a complex number Z^2 + C
}

vec2 BurningShip(vec2 z,vec2 c)
{
    z = abs(z);
    return complexSquare(z)+ c; //z is a complex number Z^2 + C
}

vec2 Tricorn(vec2 z ,vec2 c )
{
    z *= vec2(1,-1);    
    return complexSquare(z)+ c; //z is a complex number Z^2 + C
}

vec2 Feather(vec2 z,vec2 c)
{
    return complexDiv(complexCube(z) , (vec2(1,0) + (z*z))) + c;
}

vec2 Eye(vec2 z, vec2 c)
{
   return complexSquare(complexDiv(z,c ))+ c; //(z/c)^2 - c
}


vec4 fractal(vec2 C)
{
    vec2 coords = vec2(0);      
    int iter = 0;
    int maxIter = maxIteration;
    //maxIter = int(min(iTime / 10, 1200));
    float maxDot = 4.0;
    if((cFractal & 8) > 0 ||(cFractal & 16) >0)
    maxDot = 2000.0;
    
    while(dot(coords,coords)<=maxDot && iter < maxIter)
    {    
        if((cFractal & 1) == 1)
            coords = mandelbrot(coords,C );
        if((cFractal & 2) == 2)
            coords = BurningShip(coords,C );
        if((cFractal & 4) == 4)
            coords = Tricorn(coords,C );
        if((cFractal & 8) == 8)
            coords = Feather(coords,C );        
        if((cFractal & 16) == 16)
            coords = Eye(coords,C ); 
        iter++;
    } 
    if(coords ==vec2(6.9,420)) //To skip the main bulb or any other similar things
        iter = maxIter;
    return GetColor(C,float(iter) , float(maxIter));
}

float rand(float s){return fract(sin(s*12.9898) * 43758.5453);}

void main( )
{   
    vec2 uv = gl_FragCoord.xy - (iResolution.xy *.5);
    int AA = 1;
    vec4 col;
    for(int i = 0; i< AA; i++)
    {
        vec2 dxy = vec2(rand(i*.54321 + Time),rand(i*0.12345 + Time)  );        
        vec2 c = vec2((uv + dxy) * vec2(1.0,-1.0) /zoom- iMouse.xy );        
        col += fractal(c);    
    }
    col /= AA;

    FragColor = vec4(clamp(col.xyz,0,1),1.0/(Time + 1.0));
};