using System;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;

namespace Rubik_s_Cube
{
    public class Scene
    {
        //  The projection, view and model matrices.
        public mat4 projectionMatrix;
        mat4 viewMatrix;
        mat4 modelMatrix;

        //  Constants that specify the attribute indexes.
        const uint attributeIndexPosition = 0;
        const uint attributeIndexColour = 1;

        //  The vertex buffer array which contains the vertex and colour buffers.
        VertexBufferArray vertexBufferArray;
    
        //  The shader program for our vertex and fragment shader.
        private ShaderProgram shaderProgram;

        /// <summary>
        /// Initialises the scene.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        /// <param name="width">The width of the screen.</param>
        /// <param name="height">The height of the screen.</param>
        public void Initialise(OpenGL gl)
        {
            //  Set a white clear colour.
            gl.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);

            //  Create the shader program.
            var vertexShaderSource = ManifestResourceLoader.LoadTextFile("Shaders.Shader.vert");
            var fragmentShaderSource = ManifestResourceLoader.LoadTextFile("Shaders.Shader.frag");

            shaderProgram = new ShaderProgram();
            shaderProgram.Create(gl, vertexShaderSource, fragmentShaderSource, null);
            shaderProgram.BindAttributeLocation(gl, attributeIndexPosition, "in_Position");
            shaderProgram.BindAttributeLocation(gl, attributeIndexColour, "in_Color");
            shaderProgram.AssertValid(gl);

            //  Create a perspective projection matrix.
            //projectionMatrix = glm.perspective(glm.radians(45.0f), width / height, 0.1f, 100.0f);

            //  Create a view matrix to move us back a bit.
            viewMatrix = glm.translate(new mat4(1.0f), new vec3(0.0f, 0.0f, -1.0f));

            //  Create a model matrix to make the model a little bigger.
            modelMatrix = glm.scale(new mat4(1.0f), new vec3(0.5f));

            //  Now create the geometry for the square.
            CreateVerticesForSquare(gl);
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        public void Draw(OpenGL gl)
        {
            //  Clear the scene.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            //  Bind the shader, set the matrices.
            shaderProgram.Bind(gl);
            shaderProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            shaderProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());
            shaderProgram.SetUniformMatrix4(gl, "modelMatrix", modelMatrix.to_array());

            //  Bind the out vertex array.
            vertexBufferArray.Bind(gl);

            //  Draw the square.
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6); 

            //  Unbind our vertex array and shader.
            vertexBufferArray.Unbind(gl);
            shaderProgram.Unbind(gl);
        }
        
        /// <summary>
        /// Creates the geometry for the square, also creating the vertex buffer array.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        private void CreateVerticesForSquare(OpenGL gl)
        {
            var vertices = new float[18];
            var colors = new float[18]; // Colors for our vertices  
            vertices[0] = -0.5f; vertices[1] = -0.5f; vertices[2] = 0.0f; // Bottom left corner  
            colors[0] = 1.0f; colors[1] = 1.0f; colors[2] = 1.0f; // Bottom left corner  
            vertices[3] = -0.5f; vertices[4] = 0.5f; vertices[5] = 0.0f; // Top left corner  
            colors[3] = 1.0f; colors[4] = 0.0f; colors[5] = 0.0f; // Top left corner  
            vertices[6] = 0.5f; vertices[7] = 0.5f; vertices[8] = 0.0f; // Top Right corner  
            colors[6] = 0.0f; colors[7] = 1.0f; colors[8] = 0.0f; // Top Right corner  
            vertices[9] = 0.5f; vertices[10] = -0.5f; vertices[11] = 0.0f; // Bottom right corner  
            colors[9] = 0.0f; colors[10] = 0.0f; colors[11] = 1.0f; // Bottom right corner  
            vertices[12] = -0.5f; vertices[13] = -0.5f; vertices[14] = 0.0f; // Bottom left corner  
            colors[12] = 1.0f; colors[13] = 1.0f; colors[14] = 1.0f; // Bottom left corner  
            vertices[15] = 0.5f; vertices[16] = 0.5f; vertices[17] = 0.0f; // Top Right corner  
            colors[15] = 0.0f; colors[16] = 1.0f; colors[17] = 0.0f; // Top Right corner  
            
            //  Create the vertex array object.
            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            //  Create a vertex buffer for the vertex data.
            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, vertices, false, 3);

            //  Now do the same for the colour data.
            var colourDataBuffer = new VertexBuffer();
            colourDataBuffer.Create(gl);
            colourDataBuffer.Bind(gl);
            colourDataBuffer.SetData(gl, 1, colors, false, 3);

            //  Unbind the vertex array, we've finished specifying data for it.
            vertexBufferArray.Unbind(gl);
        }

        public vec2 LocalToScreen(vec4 input, vec2 viewportSize)
        {
            //var clip = projectionMatrix * viewMatrix * modelMatrix * input;
            var clip = input;
            clip /= clip.w;
            //clip = glm.normalize(clip);
            return new vec2(0.5f * (clip.x + 1.0f) * viewportSize.x, viewportSize.y - 0.5f * (clip.y + 1.0f) * viewportSize.y);
        }

        public vec4 LocalToNDC(vec4 input)
        {
            return new vec4(projectionMatrix * viewMatrix * modelMatrix * input);
        }

        public vec4 ScreenToNDC(vec2 input, vec2 viewportSize)
        {
            float x = (input.x / viewportSize.x) * 2f - 1f;
            float y = (1f - (input.y / viewportSize.y)) * 2f - 1f;
            return new vec4(x, y, 0f, 1f);
        }

        public string func(vec2 mousePos, vec2 viewportSize)
        {
            string ret = "";

            vec2 topLeft = LocalToScreen(new vec4(-0.5f, 0.5f, 0.0f, 1.0f), viewportSize);
            vec2 topRight = LocalToScreen(new vec4(0.5f, 0.5f, 0.0f, 1.0f), viewportSize);
            vec2 bottomLeft = LocalToScreen(new vec4(-0.5f, -0.5f, 0.0f, 1.0f), viewportSize);
            vec2 bottomRight = LocalToScreen(new vec4(0.5f, -0.5f, 0.0f, 1.0f), viewportSize);
            
            if (
                mousePos.x >= topLeft.x &&
                mousePos.x <= bottomRight.x &&
                mousePos.y >= topLeft.y &&
                mousePos.y <= bottomRight.y
                )
                ret = "Inside square";
            else
                ret = "Outside square";

            ret = ret + Environment.NewLine + "Length to the nearest vertex from mouse click position: ";
            float minLength = Math.Min(vecLen(topLeft - mousePos), Math.Min(vecLen(topRight - mousePos), Math.Min(vecLen(bottomLeft - mousePos), vecLen(bottomRight - mousePos))));
            ret = ret + minLength.ToString();

            ret = ret + Environment.NewLine + "Mouse: " + mousePos.x.ToString() + "; " + mousePos.y.ToString();
            ret = ret + Environment.NewLine + "Top left: " + topLeft.x.ToString() + "; " + topLeft.y.ToString();

            return ret;
        }

        public string func2(vec2 mouseScreen, vec2 viewportSize)
        {
            string ret = "";

            vec4 topLeft = LocalToNDC(new vec4(-0.5f, 0.5f, 0.0f, 1.0f));
            vec4 topRight = LocalToNDC(new vec4(0.5f, 0.5f, 0.0f, 1.0f));
            vec4 bottomLeft = LocalToNDC(new vec4(-0.5f, -0.5f, 0.0f, 1.0f));
            vec4 bottomRight = LocalToNDC(new vec4(0.5f, -0.5f, 0.0f, 1.0f));

            vec4 mousePos = ScreenToNDC(mouseScreen, viewportSize);

            if (
                mousePos.x >= topLeft.x &&
                mousePos.x <= bottomRight.x &&
                mousePos.y >= topLeft.y &&
                mousePos.y <= bottomRight.y
                )
                ret = "Inside square";
            else
                ret = "Outside square";
                       
            ret = ret + Environment.NewLine + "Mouse: " + mousePos.x.ToString() + "; " + mousePos.y.ToString();
            ret = ret + Environment.NewLine + "Top left: " + topLeft.x.ToString() + "; " + topLeft.y.ToString();

            return ret;
        }

        float vecLen(vec2 v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
        }
    }
}
