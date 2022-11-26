using CGA.Command;
using CGA.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Windows;
using CGA1.Model;

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
        private ColorBuffer _normalsTexture;
        private ColorBuffer _diffuseTexture;
        private ColorBuffer _specularTexture;
        private ColorBuffer _emissionTexture;
        private Color _color;
        private PainterType _painterType;
        private LightingType _lightingType;

        public MainWindowViewModel()
        {
            Width = 650;
            Height = 550;
            ColorBuffer = new ColorBuffer(Width, Height);
            Bitmap = CreateBitmap(Width, Height);
            Fov = 60;
            ModelScale = 0.5f;
            CameraPosZ = 10;
            Color = Color.FromRgb(128, 128, 128);
            PainterType = PainterType.Bresenham;
            LightingType = LightingType.Lambert;
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
        public ColorBuffer NormalsTexture { get => _normalsTexture; set => SetProperty(ref _normalsTexture, value, nameof(NormalsTexture)); }
        public ColorBuffer DiffuseTexture { get => _diffuseTexture; set => SetProperty(ref _diffuseTexture, value, nameof(DiffuseTexture)); }
        public ColorBuffer SpecularTexture { get => _specularTexture; set => SetProperty(ref _specularTexture, value, nameof(SpecularTexture)); }
        public ColorBuffer EmissionTexture { get => _emissionTexture; set => SetProperty(ref _emissionTexture, value, nameof(EmissionTexture)); }
        public Color Color { get => _color; set => SetProperty(ref _color, value, nameof(Color)); }
        public PainterType PainterType { get => _painterType; set => SetProperty(ref _painterType, value, nameof(PainterType)); }
        public LightingType LightingType { get => _lightingType; set => SetProperty(ref _lightingType, value, nameof(LightingType)); }

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
                case nameof(PainterType):
                case nameof(LightingType):
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
                    try
                    {
                        Obj = ObjParser.Parse(reader);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
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

            switch (PainterType)
            {
                case PainterType.Bresenham:
                    var bresenham = new Bresenham(model, ColorBuffer, Color);
                    bresenham.DrawModel();
                    break;
                case PainterType.FlatShading:
                    var lightning = GetLighting();
                    var flatShading = new FlatShading(model, ColorBuffer, Color, lightning);
                    flatShading.DrawModel();
                    break;
                case PainterType.PhongShading:
                    var lighting = GetPhongLighting();
                    var phongShading = new PhongShading(model, ColorBuffer, Color, lighting,
                        NormalsTexture, DiffuseTexture, EmissionTexture, SpecularTexture, modelMatrix);
                    phongShading.DrawModel();
                    break;
            }
            NotifyPropertyChanged(nameof(ColorBuffer));
        }

        private ILighting GetLighting()
        {
            switch (LightingType)
            {
                case LightingType.Lambert:
                    return new LambertLighting(new Vector3(0, 0, 1));
                case LightingType.Phong:
                    return GetPhongLighting();
                default:
                    return null;
            }
        }

        private PhongLighting GetPhongLighting()
        {
            var pos = new Vector3(0, 0, 1);
            var direction = -new Vector3(CameraPosX, CameraPosY, CameraPosZ);
            var backgroundFactor = new Vector3(0.3f, 0.3f, 0.3f);
            var diffuseFactor = new Vector3(1.0f, 1.0f, 1.0f);
            var mirrorFactor = new Vector3(0.3f, 0.3f, 0.3f);
            var ambientColor = new Vector3(255.0f, 255.0f, 0.0f);
            var reflectionColor = new Vector3(255.0f, 255.0f, 255.0f);
            var shinessFactor = 32.0f;
            return new PhongLighting(pos, direction, backgroundFactor, diffuseFactor,
                mirrorFactor, ambientColor, reflectionColor, shinessFactor);
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