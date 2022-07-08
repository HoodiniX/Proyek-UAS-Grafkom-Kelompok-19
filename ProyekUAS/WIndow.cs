using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace ProyekUAS
{

    static class Constants
    {
        public const string path = "../../../Shaders/";
        public const string texturePath = "../../../textures/";
    }
    internal class Window : GameWindow
    {
        List<Asset3d> objectList = new List<Asset3d>();
        Asset2d[] _object = new Asset2d[6];
        Asset3d[] _object3d = new Asset3d[30];
        private Background background;
        Asset3d light;
        double _time;
        float degr = 0;
        Camera _camera;
        bool _firstMove = true;
        Vector2 _lastPos;
        Vector3 _objectPos = new Vector3(0, 0, 0);
        float _rotationSpeed = 1f;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        private readonly Vector3[] _pointLightPositions =
        {
            //cafetaria
            new Vector3(0.75f, 0.6f, 0.0f)
        };

        protected override void OnLoad()
        {
            base.OnLoad();
            //Background Color
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            _object3d[0] = new Asset3d();
            _object3d[1] = new Asset3d();
            _object3d[2] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0, 0.5f, 0));
            _object3d[3] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0, 0.8f, 0));
            _object3d[4] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0, 0.8f, 0));
            _object3d[5] = new Asset3d();
            _object3d[6] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.9f, 0.9f, 0.9f));
            _object3d[7] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.66274509803f, 0.662745098039f, 0.66274509803f));
            _object3d[8] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.66274509803f, 0.662745098039f, 0.66274509803f));
            _object3d[9] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.66274509803f, 0.662745098039f, 0.66274509803f));
            _object3d[10] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.66274509803f, 0.662745098039f, 0.66274509803f));
            _object3d[11] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(1.0f, 1.0f, 1.0f));
            _object3d[12] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(1.0f, 1.0f, 1.0f));
            _object3d[13] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(1.0f, 1.0f, 1.0f));
            _object3d[14] = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(1.0f, 1.0f, 1.0f));
            _object3d[15] = new Asset3d();
            _object3d[16] = new Asset3d();
            _object3d[17] = new Asset3d();
            _object3d[18] = new Asset3d();
            _object3d[19] = new Asset3d();
            _object3d[20] = new Asset3d();
            _object3d[21] = new Asset3d();
            _object3d[22] = new Asset3d();
            _object3d[23] = new Asset3d();
            _object3d[24] = new Asset3d();
            _object3d[25] = new Asset3d();
            _object3d[26] = new Asset3d();
            _object3d[27] = new Asset3d();
            _object3d[28] = new Asset3d();
            _object3d[29] = new Asset3d();
            _camera = new Camera(new Vector3(0, 0, 1), Size.X / (float)Size.Y);

            _object3d[0].createWheels(0.5f, 0.2f, 0.125f, 0.0f, -0.72f, 0.0f, 72, 3);
            _object3d[0].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);

            _object3d[1].createWheels(0.5f, 0.2f, 0.125f, 0.0f, -0.72f, 0.57f, 72, 3);
            _object3d[1].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);

            _object3d[2].createTrapezoidBodyVertices(0.0f, -0.35f, 0.35f, 0.4f, true);
            _object3d[2].load2();

            _object3d[3].createTrapezoidHeadVertices(0.0f, -0.18f, 0.35f, 0.275f, true);
            _object3d[3].load2();

            _object3d[4].createLongBoxVertices(0.3f, -0.20f, 0.35f, 0.11f, true);
            _object3d[4].load2();

            _object3d[5].createEllipsoid2(0.05f, 0.02f, 0.02f, 0.5f, -0.20f, 0.35f, 72, 24);
            _object3d[5].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);

            _object3d[6].createFlatBoxVertices(0, 1.5f, 0, 0.5f, true);
            _object3d[6].load2();

            _object3d[7].createLongBoxVertices(0.45f, 1.48f, 0.2f, 0.11f, true);
            _object3d[7].load2();

            _object3d[8].createLongBoxVertices(0.45f, 1.48f, -0.2f, 0.11f, true);
            _object3d[8].load2();

            _object3d[9].createLongBoxVertices(-0.45f, 1.48f, 0.2f, 0.11f, true);
            _object3d[9].load2();

            _object3d[10].createLongBoxVertices(-0.45f, 1.48f, -0.2f, 0.11f, true);
            _object3d[10].load2();

            _object3d[11].createBoxVertices(-0.63f, 1.525f, -0.2f, 0.035f, true);
            _object3d[11].load2();

            _object3d[12].createBoxVertices(-0.63f, 1.525f, 0.2f, 0.035f, true);
            _object3d[12].load2();

            _object3d[13].createBoxVertices(0.63f, 1.525f, -0.2f, 0.035f, true);
            _object3d[13].load2();

            _object3d[14].createBoxVertices(0.63f, 1.525f, 0.2f, 0.035f, true);
            _object3d[14].load2();

            _object3d[15].createEllipsoid2(0.015f, 0.015f, 0.015f, 0.63f, 1.54f, 0.2f, 72, 24);
            _object3d[15].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[16].createEllipsoid2(0.015f, 0.015f, 0.015f, 0.63f, 1.54f, -0.2f, 72, 24);
            _object3d[16].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[17].createEllipsoid2(0.015f, 0.015f, 0.015f, -0.63f, 1.54f, 0.2f, 72, 24);
            _object3d[17].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[18].createEllipsoid2(0.015f, 0.015f, 0.015f, -0.63f, 1.54f, -0.2f, 72, 24);
            _object3d[18].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[19].createFlatLongBoxVertices(0.63f, 1.55f, 0.2f, 0.05f);
            _object3d[19].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[20].createFlatLongBoxVertices(0.63f, 1.55f, -0.2f, 0.05f);
            _object3d[20].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[21].createFlatLongBoxVertices(-0.63f, 1.55f, 0.2f, 0.05f);
            _object3d[21].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[22].createFlatLongBoxVertices(-0.63f, 1.55f, -0.2f, 0.05f);
            _object3d[22].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[23].createCone(0.3f, 0.8f, 0.3f, 1.2f, 1.49f, 0.0f, 72, 3);
            _object3d[23].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[24].createTube(0.3f, 1.0f, 0.3f, 1.2f, -0.2f, 0.0f, 72, 3);
            _object3d[24].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[25].createFrontPrism(1.2f, 0.1f, 0.38f, 1.0f);
            _object3d[25].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[26].createBackPrism(1.2f, 0.1f, -0.38f, 1.0f);
            _object3d[26].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[27].createLeftPrism(0.82f, 0.1f, 0.0f, 1.0f);
            _object3d[27].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[28].createRightPrism(1.58f, 0.1f, 0.0f, 1.0f);
            _object3d[28].load(Constants.path + "shader.vert", Constants.path  + "shader.frag", Size.X, Size.Y);

            _object3d[23].rotate(new Vector3(1.2f, -0.2f, 0.0f), _object3d[23]._euler[0], -130f);
            _object3d[24].rotate(new Vector3(1.2f, -0.2f, 0.0f), _object3d[24]._euler[0], -130f);
            _object3d[25].rotate(new Vector3(1.2f, -0.2f, 0.0f), _object3d[25]._euler[0], -130f);
            _object3d[26].rotate(new Vector3(1.2f, -0.2f, 0.0f), _object3d[26]._euler[0], -130f);
            _object3d[27].rotate(new Vector3(1.2f, -0.2f, 0.0f), _object3d[27]._euler[0], -130f);
            _object3d[28].rotate(new Vector3(1.2f, -0.2f, 0.0f), _object3d[28]._euler[0], -130f);

            var cube1 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.339f, 0.253f, 0.183f));
            cube1.createCuboid(0, -5.9f, 0, 10, true, false);
            objectList.Add(cube1);

            var trunk = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.4f, 0.3f, 0.2f));
            trunk.createVerticalLongBoxVertices(0, -0.3f, -1, 0.3f, true);
            objectList.Add(trunk);

            var leaf1 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf1.createCuboid(0, -0.3f, -1, 0.4f, true, false);
            objectList.Add(leaf1);

            var leaf2 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf2.createCuboid(0, 0.05f, -1, 0.3f, true, false);
            objectList.Add(leaf2);

            var leaf3 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf3.createCuboid(0, 0.3f, -1, 0.2f, true, false);
            objectList.Add(leaf3);

            var trunk2 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.4f, 0.3f, 0.2f));
            trunk2.createVerticalLongBoxVertices(1.3f, -0.3f, -1.3f, 0.3f, true);
            objectList.Add(trunk2);

            var leaf4 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf4.createCuboid(1.3f, -0.3f, -1.3f, 0.4f, true, false);
            objectList.Add(leaf4);

            var leaf5 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf5.createCuboid(1.3f, 0.05f, -1.3f, 0.3f, true, false);
            objectList.Add(leaf5);

            var leaf6 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf6.createCuboid(1.3f, 0.3f, -1.3f, 0.2f, true, false);
            objectList.Add(leaf6);

            var trunk3 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.4f, 0.3f, 0.2f));
            trunk3.createVerticalLongBoxVertices(0, -0.3f, 1.5f, 0.3f, true);
            objectList.Add(trunk3);

            var leaf7 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf7.createCuboid(0, -0.3f, 1.5f, 0.4f, true, false);
            objectList.Add(leaf7);

            var leaf8 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf8.createCuboid(0, 0.05f, 1.5f, 0.3f, true, false);
            objectList.Add(leaf8);

            var leaf9 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf9.createCuboid(0, 0.3f, 1.5f, 0.2f, true, false);
            objectList.Add(leaf9);

            var trunk4 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.4f, 0.3f, 0.2f));
            trunk4.createVerticalLongBoxVertices(-1, -0.3f, 0.5f, 0.3f, true);
            objectList.Add(trunk4);

            var leaf10 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf10.createCuboid(-1, -0.3f, 0.5f, 0.4f, true, false);
            objectList.Add(leaf10);

            var leaf11 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf11.createCuboid(-1, 0.05f, 0.5f, 0.3f, true, false);
            objectList.Add(leaf11);

            var leaf12 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf12.createCuboid(-1, 0.3f, 0.5f, 0.2f, true, false);
            objectList.Add(leaf12);

            var trunk5 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.4f, 0.3f, 0.2f));
            trunk5.createVerticalLongBoxVertices(-2, -0.3f, -0.3f, 0.3f, true);
            objectList.Add(trunk5);

            var leaf13 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf13.createCuboid(-2, -0.3f, -0.3f, 0.4f, true, false);
            objectList.Add(leaf13);

            var leaf14 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf14.createCuboid(-2, 0.05f, -0.3f, 0.3f, true, false);
            objectList.Add(leaf14);

            var leaf15 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf15.createCuboid(-2, 0.3f, -0.3f, 0.2f, true, false);
            objectList.Add(leaf15);

            var trunk6 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.4f, 0.3f, 0.2f));
            trunk6.createVerticalLongBoxVertices(2, -0.3f, 0.2f, 0.3f, true);
            objectList.Add(trunk6);

            var leaf16 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf16.createCuboid(2, -0.3f, 0.2f, 0.4f, true, false);
            objectList.Add(leaf16);

            var leaf17 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf17.createCuboid(2, 0.05f, 0.2f, 0.3f, true, false);
            objectList.Add(leaf17);

            var leaf18 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.0f, 1.0f, 0.0f));
            leaf18.createCuboid(2, 0.3f, 0.2f, 0.2f, true, false);
            objectList.Add(leaf18);

            var rock = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0.66274509803f, 0.662745098039f, 0.66274509803f));
            rock.createRock(2.1f, -0.65f, -0.8f, 0.5f, true);
            objectList.Add(rock);

            rock.createRock(1.1f, -0.65f, 1.4f, 0.5f, true);
            objectList.Add(rock);

            rock.createRock(0.6f, -0.65f, -1.4f, 0.5f, true);
            objectList.Add(rock);

            rock.createRock(-0.9f, -0.65f, -0.6f, 0.5f, true);
            objectList.Add(rock);

            light = new Asset3d("shader.vert", "shader.frag", new Vector3(0.96f, 0.29f, 0.007f));
            light.createCuboid(1.2f, -0.85f, -0.6f, 0.05f, false, false);

            var backgroundPaths = new List<string>();
            backgroundPaths.Add(Constants.texturePath + "right.png");
            backgroundPaths.Add(Constants.texturePath + "left.png");
            backgroundPaths.Add(Constants.texturePath + "top.png");
            backgroundPaths.Add(Constants.texturePath + "bottom.png");
            backgroundPaths.Add(Constants.texturePath + "front.png");
            backgroundPaths.Add(Constants.texturePath + "back.png");

            background = new Background("background.vert", "background.frag", backgroundPaths);
            background.load();

            light.load2();

            foreach (Asset3d i in objectList)
            {
                i.load2();
            }

            CursorGrabbed = true;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _time += 9.0 * args.Time;
            Matrix4 temp = Matrix4.Identity;
            //temp = temp * Matrix4.CreateTranslation(0.5f, 0.5f, 0.5f);
            //degr += MathHelper.DegreesToRadians(1f);
            //temp = temp * Matrix4.CreateRotationX(degr);

            _object3d[0].render(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[1].render(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[2].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[3].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[4].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[5].render(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[6].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[7].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[8].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[9].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[10].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[11].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[12].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[13].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[14].render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            _object3d[15].render(new Vector4(0.66274509803f, 0.662745098039f, 0.66274509803f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[16].render(new Vector4(0.66274509803f, 0.662745098039f, 0.66274509803f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[17].render(new Vector4(0.66274509803f, 0.662745098039f, 0.66274509803f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[18].render(new Vector4(0.66274509803f, 0.662745098039f, 0.66274509803f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[19].render(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[20].render(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[21].render(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[22].render(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[23].render(new Vector4(0.8627f, 0.0784313f, 0.235294117647f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[24].render(new Vector4(0.66274509803f, 0.662745098039f, 0.66274509803f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[25].render(new Vector4(0.8627f, 0.0784313f, 0.235294117647f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[26].render(new Vector4(0.8627f, 0.0784313f, 0.235294117647f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[27].render(new Vector4(0.8627f, 0.0784313f, 0.235294117647f, 1.0f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            _object3d[28].render(new Vector4(0.8627f, 0.0784313f, 0.235294117647f, 0.5f), 3, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());

            _object3d[19].rotate(new Vector3(0.63f, 1.55f, 0.2f), _object3d[19]._euler[1], 15f);
            _object3d[20].rotate(new Vector3(0.63f, 1.55f, -0.2f), _object3d[20]._euler[1], 15f);
            _object3d[21].rotate(new Vector3(-0.63f, 1.55f, 0.2f), _object3d[21]._euler[1], 15f);
            _object3d[22].rotate(new Vector3(-0.63f, 1.55f, -0.2f), _object3d[22]._euler[1], 15f);

            /*_object3d[23].rotate(new Vector3(0.7f, 3.0f, 0.8f), _object3d[23]._euler[0], 2f);
            _object3d[24].rotate(new Vector3(0.7f, 3.0f, 0.8f), _object3d[24]._euler[0], 2f);
            _object3d[25].rotate(new Vector3(0.7f, 3.0f, 0.8f), _object3d[25]._euler[0], 2f);
            _object3d[26].rotate(new Vector3(0.7f, 3.0f, 0.8f), _object3d[26]._euler[0], 2f);
            _object3d[27].rotate(new Vector3(0.7f, 3.0f, 0.8f), _object3d[27]._euler[0], 2f);
            _object3d[28].rotate(new Vector3(0.7f, 3.0f, 0.8f), _object3d[28]._euler[0], 2f);*/

            light.render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            //light.rotate(new Vector3(0.0f, 0.0f, 0.0f), _object3d[0]._euler[0], 0.5f);

            foreach (Asset3d i in objectList)
            {
                i.render2(1, _camera.GetViewMatrix(), _camera.GetProjectionMatrix(), _camera.Position, light);
            }

            GL.Disable(EnableCap.CullFace);
            GL.DepthFunc(DepthFunction.Lequal);

            background.render(_camera.GetViewMatrix(), _camera.GetProjectionMatrix());

            GL.DepthFunc(DepthFunction.Less);
            //GL.Enable(EnableCap.CullFace);

            SwapBuffers();
        }

        public Matrix4 generateArbRotationMatrix(Vector3 axis, Vector3 center, float degree)
        {
            var rads = MathHelper.DegreesToRadians(degree);

            var secretFormula = new float[4, 4] {
                { (float)Math.Cos(rads) + (float)Math.Pow(axis.X, 2) * (1 - (float)Math.Cos(rads)), axis.X* axis.Y * (1 - (float)Math.Cos(rads)) - axis.Z * (float)Math.Sin(rads),    axis.X * axis.Z * (1 - (float)Math.Cos(rads)) + axis.Y * (float)Math.Sin(rads),   0 },
                { axis.Y * axis.X * (1 - (float)Math.Cos(rads)) + axis.Z * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Y, 2) * (1 - (float)Math.Cos(rads)), axis.Y * axis.Z * (1 - (float)Math.Cos(rads)) - axis.X * (float)Math.Sin(rads),   0 },
                { axis.Z * axis.X * (1 - (float)Math.Cos(rads)) - axis.Y * (float)Math.Sin(rads),   axis.Z * axis.Y * (1 - (float)Math.Cos(rads)) + axis.X * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Z, 2) * (1 - (float)Math.Cos(rads)), 0 },
                { 0, 0, 0, 1}
            };
            var secretFormulaMatix = new Matrix4
            (
                new Vector4(secretFormula[0, 0], secretFormula[0, 1], secretFormula[0, 2], secretFormula[0, 3]),
                new Vector4(secretFormula[1, 0], secretFormula[1, 1], secretFormula[1, 2], secretFormula[1, 3]),
                new Vector4(secretFormula[2, 0], secretFormula[2, 1], secretFormula[2, 2], secretFormula[2, 3]),
                new Vector4(secretFormula[3, 0], secretFormula[3, 1], secretFormula[3, 2], secretFormula[3, 3])
            );

            return secretFormulaMatix;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _camera.Fov = _camera.Fov - e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var input = KeyboardState;
            var mouse_input = MouseState;
            var sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            float cameraSpeed = 0.5f;
            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)args.Time;
            }

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse_input.X, mouse_input.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse_input.X - _lastPos.X;
                var deltaY = mouse_input.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse_input.X, mouse_input.Y);
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }

            if (KeyboardState.IsKeyDown(Keys.N))
            {
                var axis = new Vector3(0, 1, 0);
                _camera.Position -= _objectPos;
                _camera.Position = Vector3.Transform(_camera.Position, generateArbRotationMatrix(axis, _objectPos, _rotationSpeed).ExtractRotation());
                _camera.Position += _objectPos;
                _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
            }
            if (KeyboardState.IsKeyDown(Keys.Comma))
            {
                var axis = new Vector3(0, 1, 0);
                _camera.Position -= _objectPos;
                _camera.Yaw -= _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position, generateArbRotationMatrix(axis, _objectPos, -_rotationSpeed).ExtractRotation());
                _camera.Position += _objectPos;
                _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
            }
            if (KeyboardState.IsKeyDown(Keys.K))
            {
                var axis = new Vector3(1, 0, 0);
                _camera.Position -= _objectPos;
                _camera.Pitch -= _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position, generateArbRotationMatrix(axis, _objectPos, _rotationSpeed).ExtractRotation());
                _camera.Position += _objectPos;
                _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
            }
            if (KeyboardState.IsKeyDown(Keys.M))
            {
                var axis = new Vector3(1, 0, 0);
                _camera.Position -= _objectPos;
                _camera.Pitch += _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position, generateArbRotationMatrix(axis, _objectPos, -_rotationSpeed).ExtractRotation());
                _camera.Position += _objectPos;
                _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButton.Left)
            {
                float _x = (MousePosition.X - Size.X / 2) / (Size.X / 2);
                float _y = -(MousePosition.Y - Size.Y / 2) / (Size.Y / 2);
            }
        }
    }
}
