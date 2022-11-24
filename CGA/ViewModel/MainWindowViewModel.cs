using CGA.Command;
using CGA.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Numerics;

namespace CGA.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _width;
        private int _height;

        private ColorBuffer _colorBuffer;
        private WriteableBitmap _bitmap;

        private float _fov;

        private float _modelPosX;
        private float _modelPosY;
        private float _modelPosZ;

        private float _modelYaw;
        private float _modelPitch;
        private float _modelRoll;

        private float _modelScale;

        private float _cameraPosX;
        private float _cameraPosY;
        private float _cameraPosZ;

        private float _cameraYaw;
        private float _cameraPitch;
        private float _cameraRoll;

        private Obj _obj;

        private Color _color;

        public MainWindowViewModel()
        {
            Width = 650;
            Height = 550;
            ColorBuffer = new ColorBuffer(Width, Height);
            Bitmap = CreateBitmap(Width, Height);
            Fov = 60;
            ModelScale = 0.5f;
            CameraPosZ = 10;
            Color = Colors.Black;
            FileDialog = new OpenFileDialog
            {
                Filter = "Object | *.obj",
                Title = "Select obj file",
                Multiselect = false
            };
            LoadObjCommand = new DelegateCommand(o => LoadObj());
            PropertyChanged += OnPropertyChanged;
        }

        public int Width { get => _width; set => SetProperty(ref _width, value, nameof(Width)); }
        public int Height { get => _height; set => SetProperty(ref _height, value, nameof(Height)); }

        public ColorBuffer ColorBuffer { get => _colorBuffer; set => SetProperty(ref _colorBuffer, value, nameof(ColorBuffer)); }
        public WriteableBitmap Bitmap { get => _bitmap; set => SetProperty(ref _bitmap, value, nameof(Bitmap)); }

        public float Fov { get => _fov; set => SetProperty(ref _fov, value, nameof(Fov)); }

        public float ModelPosX { get => _modelPosX; set => SetProperty(ref _modelPosX, value, nameof(ModelPosX)); }
        public float ModelPosY { get => _modelPosY; set => SetProperty(ref _modelPosY, value, nameof(ModelPosY)); }
        public float ModelPosZ { get => _modelPosZ; set => SetProperty(ref _modelPosZ, value, nameof(ModelPosZ)); }

        public float ModelYaw { get => _modelYaw; set => SetProperty(ref _modelYaw, value, nameof(ModelYaw)); }
        public float ModelPitch { get => _modelPitch; set => SetProperty(ref _modelPitch, value, nameof(ModelPitch)); }
        public float ModelRoll { get => _modelRoll; set => SetProperty(ref _modelRoll, value, nameof(ModelRoll)); }

        public float ModelScale { get => _modelScale; set => SetProperty(ref _modelScale, value, nameof(ModelScale)); }

        public float CameraPosX { get => _cameraPosX; set => SetProperty(ref _cameraPosX, value, nameof(CameraPosX)); }
        public float CameraPosY { get => _cameraPosY; set => SetProperty(ref _cameraPosY, value, nameof(CameraPosY)); }
        public float CameraPosZ { get => _cameraPosZ; set => SetProperty(ref _cameraPosZ, value, nameof(CameraPosZ)); }

        public float CameraYaw { get => _cameraYaw; set => SetProperty(ref _cameraYaw, value, nameof(CameraYaw)); }
        public float CameraPitch { get => _cameraPitch; set => SetProperty(ref _cameraPitch, value, nameof(CameraPitch)); }
        public float CameraRoll { get => _cameraRoll; set => SetProperty(ref _cameraRoll, value, nameof(CameraRoll)); }

        public Obj Obj { get => _obj; set => SetProperty(ref _obj, value, nameof(Obj)); }

        public Color Color { get => _color; set => SetProperty(ref _color, value, nameof(Color)); }

        public ICommand LoadObjCommand { get; }

        private FileDialog FileDialog { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, string propertyName)
        {
            if (Equals(field, newValue))
            {
                return false;
            }
            field = newValue;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Width):
                case nameof(Height):
                    Bitmap = CreateBitmap(Width, Height);
                    ColorBuffer = new ColorBuffer(Width, Height);
                    Repaint();
                    break;
                case nameof(ColorBuffer):
                    ColorBuffer.Write(Bitmap);
                    NotifyPropertyChanged(nameof(Bitmap));
                    break;
                case nameof(Fov):
                case nameof(ModelPosX):
                case nameof(ModelPosY):
                case nameof(ModelPosZ):
                case nameof(ModelYaw):
                case nameof(ModelPitch):
                case nameof(ModelRoll):
                case nameof(ModelScale):
                case nameof(CameraPosX):
                case nameof(CameraPosY):
                case nameof(CameraPosZ):
                case nameof(CameraYaw):
                case nameof(CameraPitch):
                case nameof(CameraRoll):
                case nameof(Obj):
                case nameof(Color):
                    Repaint();
                    break;
            }
        }

        private void LoadObj()
        {
            var open = FileDialog.ShowDialog() ?? false;
            if (open)
            {
                var fileName = FileDialog.FileName;
                using (var reader = new StreamReader(new FileStream(fileName, FileMode.Open)))
                {
                    Obj = ObjParser.Parse(reader);
                }
            }
        }

        private void Repaint()
        {
            if (Obj is null)
                return;

            var viewportMatrix = Matrices.CreateViewportMatrix(0, 0, Width, Height);
            var projectionMatrix = Matrices.CreateProjectionByAspect(ToRadians(Fov), (float)Width / Height, 0.1f, 100.0f);
            var viewMatrix = Matrices.CreateViewMatrix(CameraPosX, CameraPosY, CameraPosZ,
                ToRadians(CameraYaw), ToRadians(CameraPitch), ToRadians(CameraRoll));
            var modelMatrix = Matrices.CreateModelMatrix(ModelPosX, ModelPosY, ModelPosZ,
                ToRadians(ModelYaw), ToRadians(ModelPitch), ToRadians(ModelRoll), ModelScale);

            var model = Obj.Transform(viewportMatrix, projectionMatrix, viewMatrix, modelMatrix);

            ColorBuffer.Fill(Colors.White);

            var lightning = new LambertLighting(new Vector3(0, 0, 1));
            var painter = new FlatShading(model, ColorBuffer, Color, lightning);
            painter.DrawModel();

            NotifyPropertyChanged(nameof(ColorBuffer));
        }

        private static float ToRadians(float angle)
        {
            return (float)(angle / 180 * Math.PI);
        }

        private static WriteableBitmap CreateBitmap(int width, int height)
        {
            return new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }
    }
}