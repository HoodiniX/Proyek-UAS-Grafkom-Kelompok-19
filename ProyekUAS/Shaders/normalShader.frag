#version 430

out vec4 outputColor;

in vec3 normal;
in vec3 FragPos;

uniform vec4 objColor;

uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;

void main() 
{
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - FragPos);

    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * lightColor;

    float specularStrength = 1;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), 32);
    vec3 specular = specularStrength * spec * lightColor;

    vec3 result = (ambient + diffuse + specular) * vec3(objColor);

    outputColor = vec4(result, objColor.w);
}