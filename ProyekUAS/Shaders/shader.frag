#version 330

out vec4 outputColor;

in vec4 vertexColor;

uniform vec4 ourColor;

uniform vec4 objColor;

uniform vec3 lightColor;

void main()
{
	outputColor = objColor;
}