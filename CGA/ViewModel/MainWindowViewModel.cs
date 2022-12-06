using CGA.Command;
using CGA.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
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
        private PainterType _painterType;
        private LightingType _lightingType;

        private Color _ambientColor;
        private Color _diffuseColor;
        private Color _specularColor;

        private float _ambientFactor;
        private float _diffuseFactor;
        private float _specularFactor;
        private float _shinessFactor;

        private bool _isNormalTextureEnabled;
        private bool _isDiffuseTextureEnabled;
        private bool _isSpecularTextureEnabled;

        public MainWindowViewModel()
        {
            Width = 750;
            Height = 650;
            ColorBuffer = new ColorBuffer(Width, Height);
            Bitmap = Utils.CreateBitmap(Width, Height);
            Fov = 60.0f;
            ModelScale = 0.5f;
            CameraPosZ = 10.0f;
            PainterType = PainterType.Bresenham;
            LightingType = LightingType.Lambert;
            AmbientColor = Colors.DarkGray;
            DiffuseColor = Colors.Gray;
            SpecularColor = Colors.LightYellow;
            AmbientFactor = 0.1f;
            DiffuseFactor = 0.5f;
            SpecularFactor = 0.3f;
            ShinessFactor = 32.0f;
            IsNormalTextureEnabled = false;
            IsDiffuseTextureEnabled = false;
            IsSpecularTextureEnabled = false;

            var fileDialog = new OpenFileDialog
            {
                Filter = "Object | *.obj",
                Title = "Select obj file",
                Multiselect = false
            };
            LoadObjCommand = new DelegateCommand(o =>
            {
                if (fileDialog.ShowDialog().GetValueOrDefault(false))
                {
                    Obj = Utils.LoadObj(fileDialog.FileName);
                }
            });
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
        public PainterType PainterType { get => _painterType; set => SetProperty(ref _painterType, value, nameof(PainterType)); }
        public LightingType LightingType { get => _lightingType; set => SetProperty(ref _lightingType, value, nameof(LightingType)); }

        public Color AmbientColor { get => _ambientColor; set => SetProperty(ref _ambientColor, value, nameof(AmbientColor)); }
        public Color DiffuseColor { get => _diffuseColor; set => SetProperty(ref _diffuseColor, value, nameof(DiffuseColor)); }
        public Color SpecularColor { get => _specularColor; set => SetProperty(ref _specularColor, value, nameof(SpecularColor)); }

        public float AmbientFactor { get => _ambientFactor; set => SetProperty(ref _ambientFactor, value, nameof(AmbientFactor)); }
        public float DiffuseFactor { get => _diffuseFactor; set => SetProperty(ref _diffuseFactor, value, nameof(DiffuseFactor)); }
        public float SpecularFactor { get => _specularFactor; set => SetProperty(ref _specularFactor, value, nameof(SpecularFactor)); }
        public float ShinessFactor { get => _shinessFactor; set => SetProperty(ref _shinessFactor, value, nameof(ShinessFactor)); }

        public bool IsNormalTextureEnabled { get => _isNormalTextureEnabled; set => SetProperty(ref _isNormalTextureEnabled, value, nameof(IsNormalTextureEnabled)); }
        public bool IsDiffuseTextureEnabled { get => _isDiffuseTextureEnabled; set => SetProperty(ref _isDiffuseTextureEnabled, value, nameof(IsDiffuseTextureEnabled)); }
        public bool IsSpecularTextureEnabled { get => _isSpecularTextureEnabled; set => SetProperty(ref _isSpecularTextureEnabled, value, nameof(IsSpecularTextureEnabled)); }

        public ICommand LoadObjCommand { get; }

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
                    Bitmap = Utils.CreateBitmap(Width, Height);
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
                case nameof(PainterType):
                case nameof(LightingType):
                case nameof(AmbientColor):
                    Repaint();
                    break;
                case nameof(DiffuseColor):
                case nameof(SpecularColor):
                case nameof(AmbientFactor):
                case nameof(DiffuseFactor):
                case nameof(SpecularFactor):
                case nameof(ShinessFactor):
                    if (LightingType == LightingType.Phong)
                        Repaint();
                    break;

                case nameof(IsNormalTextureEnabled):
                case nameof(IsDiffuseTextureEnabled):
                case nameof(IsSpecularTextureEnabled):
                    if (LightingType == LightingType.Phong)
                        Repaint();
                    break;
            }
        }

        private void Repaint()
        {
            if (Obj is null)
                return;

            var viewportMatrix = Matrices.CreateViewportMatrix(0, 0, Width, Height);
            var projectionMatrix = Matrices.CreateProjectionByAspect(Utils.ToRadians(Fov), (float)Width / Height, 0.1f, 100.0f);
            var viewMatrix = Matrices.CreateViewMatrix(CameraPosX, CameraPosY, CameraPosZ,
                Utils.ToRadians(CameraYaw), Utils.ToRadians(CameraPitch), Utils.ToRadians(CameraRoll));
            var modelMatrix = Matrices.CreateModelMatrix(ModelPosX, ModelPosY, ModelPosZ,
                Utils.ToRadians(ModelYaw), Utils.ToRadians(ModelPitch), Utils.ToRadians(ModelRoll), ModelScale);

            var model = Obj.Transform(viewportMatrix, projectionMatrix, viewMatrix, modelMatrix);

            ColorBuffer.Fill(Colors.White);

            switch (PainterType)
            {
                case PainterType.Bresenham:
                    var bresenham = new Bresenham(model, ColorBuffer, AmbientColor);
                    bresenham.DrawModel();
                    break;
                case PainterType.FlatShading:
                    var flatShading = new FlatShading(model, ColorBuffer, AmbientColor, GetLighting());
                    flatShading.DrawModel();
                    break;
                case PainterType.PhongShading:
                    var phongShading = new PhongShading(model, ColorBuffer, AmbientColor, GetLighting(), modelMatrix);
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
                    var lightingDirection = new Vector3(0.5f);
                    return new LambertLighting(lightingDirection);
                case LightingType.Phong:
                    return GetPhongLighting();
                default:
                    throw new InvalidEnumArgumentException(nameof(LightingType));
            }
        }

        private PhongLighting GetPhongLighting()
        {
            var lightingDirection = new Vector3(0.5f);
            var viewerDirection = -new Vector3(CameraPosX, CameraPosY, CameraPosZ);
            var ambientFactor = new Vector3(AmbientFactor);
            var diffuseFactor = new Vector3(DiffuseFactor);
            var specularFactor = new Vector3(SpecularFactor);
            var diffuseColor = Utils.AsVector(DiffuseColor);
            var specularColor = Utils.AsVector(SpecularColor);
            return new PhongLighting(lightingDirection, viewerDirection, ambientFactor, diffuseFactor, specularFactor, 
                diffuseColor, specularColor, ShinessFactor, IsNormalTextureEnabled, IsDiffuseTextureEnabled, IsSpecularTextureEnabled);
        }
    }
}