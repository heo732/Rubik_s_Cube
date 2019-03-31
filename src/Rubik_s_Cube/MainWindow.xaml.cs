using System.Windows;
using System.Windows.Input;
using SharpGL.SceneGraph;
using GlmNet;

namespace Rubik_s_Cube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Scene scene = new Scene();

        private vec2 viewportSize;

        public MainWindow()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Draw the scene.
            scene.Draw(openGLControl.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //  Initialise the scene.            
            scene.Initialise(openGLControl.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            viewportSize = new vec2()
            {
                x = (float)openGLControl.RenderSize.Width,
                y = (float)openGLControl.RenderSize.Height
            };
            scene.projectionMatrix = glm.perspective(glm.radians(45.0f), viewportSize.x / viewportSize.y, 0.1f, 100.0f); ;            
        }

        public void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                containerMenu.Visibility = Visibility.Visible;
            }
        }

        public void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            containerMenu.Visibility = Visibility.Hidden;
        }

        private void OpenGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(openGLControl);
            MessageBox.Show(scene.func(new vec2((float)p.X, (float)p.Y), viewportSize));
        }
    }
}
