#version 430

//=================================================================
// ViewShaderData.glsl    
//   v1.0 2016-12-06  initial release
//   v1.1 2017-02-19  rearranged for easier coding
// Display shader data like date, time, frameCount, runtime, fps, 
//   resolution and mouse position.
// Click and drag mouse button to display current mouse position.
//=================================================================

//------------------------------------------
// GLSL Number Printing - @P_Malin
// Creative Commons CC0 1.0 Universal (CC-0) 
// https://www.shadertoy.com/view/4sBSWW
//------------------------------------------

layout(location = 0 ) uniform float iTime = 0;
layout(location = 1)  uniform vec3 iResolution;
layout(location = 3) uniform vec4 iMouse;
layout(location = 2) uniform int iFrame;

out vec4 FragColor;


#define SPACE tp.x += 33.;

vec2 vFontSize = vec2(8.0, 15.0);	// multiples of 4x5 work best

vec2 ppos = vec2(0);          // pixel position
vec2 tp = vec2(0.0);          // text position

const vec3 backColor = vec3(0.15, 0.10, 0.10);
vec3 drawColor = vec3(1.0, 1.0, 0.0);
vec3 vColor = backColor;

//----------------------------
float DigitBin(const in int x)
{
    return x==0 ? 480599.0
         : x==1 ? 139810.0
         : x==2 ? 476951.0
         : x==3 ? 476999.0
         : x==4 ? 350020.0
         : x==5 ? 464711.0
         : x==6 ? 464727.0
         : x==7 ? 476228.0
         : x==8 ? 481111.0
         : x==9 ? 481095.0
         :             0.0;
}
//---------------------------------
void WriteValue(const vec2 vPixelCoords
               ,const float fValue
               ,const int maxDigits
               ,const int decimalPlaces )
{
  vec2 vCharCoords = (ppos.xy - vPixelCoords) / vFontSize;
  float fDecimalPlaces = float(decimalPlaces);
  if ((vCharCoords.y < 0.0) || (vCharCoords.y >= 1.0)) return;
  float fLog10Value = log2(abs(fValue)) / log2(10.0);
  float fBiggestIndex = max(floor(fLog10Value), 0.0);
  float fDigitIndex = float(maxDigits) - floor(vCharCoords.x);
  float fCharBin = 0.0;
  if(fDigitIndex > (-fDecimalPlaces - 1.01)) 
  {
    if(fDigitIndex > fBiggestIndex) 
    {
	  if((fValue < 0.0) && (fDigitIndex < (fBiggestIndex+1.5))) fCharBin = 1792.0;
	}
    else 
    {
      if(fDigitIndex == -1.0) 
      {
        if(fDecimalPlaces > 0.0) fCharBin = 2.0;
      }
      else 
      {
        float fReducedRangeValue = fValue;
        if(fDigitIndex < 0.0) { fReducedRangeValue = fract( fValue ); fDigitIndex += 1.0; }
        float fDigitValue = (abs(fReducedRangeValue / (pow(10.0, fDigitIndex))));
        fCharBin = DigitBin(int(floor(mod(fDigitValue, 10.0))));
      }
    }
  }
  float cInt = floor(mod((fCharBin / pow(2.0, floor(fract(vCharCoords.x) * 4.0) 
                                + (floor(vCharCoords.y * 5.0) * 4.0))), 2.0));
  vColor = mix(vColor, drawColor, cInt);
}

//=================================================================

const vec3 mpColor   = vec3(0.99, 0.99, 0.00);
const vec3 mxColor   = vec3(1.00, 0.00, 0.00);
const vec3 myColor   = vec3(0.00, 1.00, 0.00);
      vec3 dotColor  = vec3(0.50, 0.50, 0.00);
          
//----------------------------------------------------------------
void SetColor(float red, float green, float blue)
{
  drawColor = vec3(red,green,blue);    
}
//----------------------------------------------------------------
void WriteMousePos(vec2 mPos)
{
  mPos = abs(mPos);
  int digits = 3;
  float radius = 3.0;

  // print dot at mPos
  if (iMouse.z > 0.0) dotColor = mpColor;
  float fDistToPointB = length(mPos - ppos) - radius;
  vColor += mix( vec3(0), dotColor, (1.0 - clamp(fDistToPointB, 0.0, 1.0)));

  // print mouse.x
  tp = mPos + vec2(-4.4 * vFontSize.x, radius + 4.0);
  tp.x = max(tp.x, -vFontSize.x);
  tp.x = min(tp.x, iResolution.x - 8.4*vFontSize.x);
  tp.y = max(tp.y, 1.6 * vFontSize.y);
  tp.y = min(tp.y, iResolution.y - 1.4*vFontSize.y);
  drawColor = mxColor;
  WriteValue(tp, mPos.x, digits, 0);
		
  // print 2nd mouse value
  SPACE
  drawColor = myColor;
  WriteValue(tp, mPos.y, digits, 0);
}    
//----------------------------------------------------------------
void main()
{
  float time = iTime;
  ppos = gl_FragCoord.xy;

  // print mouse position & coordinates
  WriteMousePos(iMouse.zw);  // last position
  WriteMousePos(iMouse.xy);  // current position

  // print Resolution
  tp = iResolution.xy - vec2(111, 33);   // text position: right top
  SetColor (0.8, 0.8, 0.8);
  WriteValue(tp, iResolution.x, 4, 0);
  SPACE SPACE
  WriteValue(tp, iResolution.y, 4, 0);

  

  // print Time


  // print Frame Counter
  SetColor (0.4, 0.7, 0.4);
  WriteValue(vec2(180, 5), float(iFrame)*1.0, 6, 0);

  // print Shader Time
  SetColor (0.0, 1.0, 1.0);
  WriteValue(vec2(240, 5), time, 6, 2);

  // print Frames Per Second - FPS  see https://www.shadertoy.com/view/lsKGWV
  //float fps = (1.0 / iTimeDelta + 0.5);
  
  
  

  FragColor = vec4(vColor,1.0);
}