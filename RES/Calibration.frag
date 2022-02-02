#version 430
out vec4 FragColor;

layout(location = 0 ) uniform float iTime = 0;
layout(location = 1)  uniform vec3 iResolution;


// Created by inigo quilez - iq/2021

float PrintDigit(in int n, in vec2 p)
{		
    // digit bitmap by P_Malin (https://www.shadertoy.com/view/4sf3RN)
    const int lut[10] = int[10](480599,139810,476951,476999,71028,464711,464727,476228,481111,481095);
    
	ivec2 xy = ivec2(p*vec2(4,5));
	int   id = 4*xy.y + xy.x;
    return float( (lut[n]>>id) & 1 );
}

float PrintInt(const in vec2 uv, in int value )
{
    float res = 0.0;

    if( abs(uv.y-0.5)<0.5 )
    {
        float maxDigits = (value==0) ? 1.0 : floor(1.0+log2(float(value))/log2(10.0));
        float digitID = floor(uv.x);
        if( digitID>=0.0 && digitID<maxDigits )
        {
            float digitVa = mod( floor(float(value)/pow(10.0,maxDigits-1.0-digitID) ), 10.0 );
            res = PrintDigit( int(digitVa), vec2(fract(uv.x), uv.y) );
        }
    }
	return res;	
}

float PrintIntN(const in vec2 uv, in int value, in int maxDigits )
{
    float res = 0.0;

    if( abs(uv.y-0.5)<0.5 )
    {
        int digitID = int(floor(uv.x));
        if( digitID>=0 && digitID<maxDigits )
        {
            float digitVa = mod( floor(float(value)/pow(10.0,float(maxDigits-1-digitID)) ), 10.0 );
            res = PrintDigit( int(digitVa), vec2(fract(uv.x), uv.y) );
        }
    }
	return res;	
}

//-----------------------------------------------------------------

vec3 pat1( in vec2 pixel, in vec2 res )
{
    vec2  uv = pixel/res.y;
    
    vec2 p = floor(pixel*exp2(-floor(6.0*uv.y)) );

    float col = mod( p.x + p.y, 2.0 );
    col *= smoothstep(0.005,0.010,abs(fract(6.0*uv.y+0.5)-0.5));
    
    return vec3( col );
}

vec3 pat2( in vec2 pixel, in vec2 res )
{
    vec2 uv = (2.0*pixel-res)/res.y;
    float h = res.y*0.8;
    
    vec3 col = vec3(0.2);
    
    if( pixel.y<h )
    {
        col = vec3( pixel.y/h );

        if( uv.x>0.0 )
        {
            vec2 p = floor(pixel);
            float f = mod( p.x +p.y, 2.0 );

            const float gamma = 2.3;
            float midgrey = pow( 0.5, 1.0/gamma );

            f = mix( midgrey,  f, smoothstep(0.1,0.101,length(vec2(uv.x,(fract(3.0*uv.y+0.5)-0.5)/3.0)-vec2(0.5*res.x/res.y,0.0))) );

            col = vec3(f);
        }
    }
    else
    {
        vec2 q = vec2(uv.x,uv.y-0.9);
        q = 0.707*abs(vec2(q.x+q.y,-q.y+q.x));
        q = (q.y<q.x)?q.yx:q;
        q -= vec2(0.01,0.05);
        float t = step( max(q.x,q.y), 0.0 );
        
        t += PrintInt( (uv-vec2(-0.5,0.85))*10.0, int(iResolution.x) );
        t += PrintInt( (uv-vec2( 0.1,0.85))*10.0, int(iResolution.y) );

        int ideg = int(iTime*60.0);
        int degs = (ideg   ) % 60;
        int secs = (ideg/60) % 60;
        int mins = (ideg/3600) % 60;
        t += PrintIntN( (uv-vec2(-0.3,0.67))*10.0, mins, 2 );
        t += PrintIntN( (uv-vec2( 0.0,0.67))*10.0, secs, 2 );
        t += PrintIntN( (uv-vec2( 0.3,0.67))*10.0, degs, 2 );
        
        // draw : :
        q = vec2( abs(uv.x-0.087)-0.147,abs(uv.y-0.72)-0.035);
        q = abs(q)-0.01;
        t += step( max(q.x,q.y), 0.0 );

        col = mix( col, vec3(1.0,0.5,0.0), t );
    }


    return col;
}

vec3 pat3( in vec2 pixel, in vec2 res )
{
    float v = pixel.y/res.y;
    
    int fps = 10;
    if( pixel.x>res.x*1.0/5.0 ) fps =  15;
    if( pixel.x>res.x*2.0/5.0 ) fps =  30;
    if( pixel.x>res.x*3.0/5.0 ) fps =  60;
    if( pixel.x>res.x*4.0/5.0 ) fps = 120;
    int id = int(floor(v*float(fps)));
    
    float f = 0.2+0.2*float(id&1);
    
    vec3 col = vec3(f);

    if( (int(iTime*float(fps))%fps)==id ) col = vec3(1.0);

    col *= smoothstep(0.01,0.02,abs(fract(5.0*pixel.x/res.x+0.5)-0.5));

    
    vec2 q = vec2( mod(pixel.x,res.x/5.0)-res.x/64.0,pixel.y-res.y*0.97);
    float t = PrintInt( q/res.y*50.0, fps );
    
    col = mix( col, vec3(1.0,0.5,0.0), t );
    
    return col;
}


void main()
{
    float x0 = iResolution.x*0.0/3.0;
    float x1 = iResolution.x*1.0/3.0;
    float x2 = iResolution.x*2.0/3.0;
    float wi = iResolution.x/3.0;
    
    vec3                 col = pat1(gl_FragCoord.xy-vec2(x0,0.0),vec2(wi,iResolution.y));
    if( gl_FragCoord.xy.x>x1 ) col = pat2(gl_FragCoord.xy-vec2(x1,0.0),vec2(wi,iResolution.y));
    if( gl_FragCoord.xy.x>x2 ) col = pat3(gl_FragCoord.xy-vec2(x2,0.0),vec2(wi,iResolution.y));

    col *= smoothstep( 2.0, 4.0, mod(gl_FragCoord.xy.x,wi) );

    ivec2 p = ivec2(gl_FragCoord.xy);
    ivec2 r = ivec2(iResolution);
    if( p.x==0 || p.y==0 || p.x==(r.x-1)  || p.y==(r.y-1) ) col = vec3(1.0,0.5,0.0);


	FragColor = vec4(col,1.0);   
}