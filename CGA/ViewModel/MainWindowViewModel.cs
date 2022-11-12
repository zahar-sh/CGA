using CGA1.Command;
using CGA1.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System;

namespace CGA1.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
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

        private IObjPainter _objPainter;

        private Obj _obj;

        private WritableImage _image;

        private Color _color;

        public MainWindowViewModel()
        {
            Image = new WritableImage(1, 1);
            Fov = 60;
            ObjPainter = new BresenhamPainter();
            Color = Color.FromRgb(0, 0, 0);
            FileDialog = new OpenFileDialog
            {
                Filter = "Object | *.obj",
                Title = "Select obj file",
                Multiselect = false
            };
            LoadObjCommand = new DelegateCommand(o => LoadObj());
            RepaintCommand= new DelegateCommand(o => Repaint());
        }

        public int ImageWidth
        {
            get => Image.Width;
            set
            {
                if (!Equals(ImageWidth, value))
                {
                    Image = new WritableImage(value, ImageWidth);
                }
            }
        }
        public int ImageHeight
        {
            get => Image.Height;
            set
            {
                if (!Equals(ImageHeight, value))
                {
                    Image = new WritableImage(ImageWidth, value);
                }
            }
        }

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

        public IObjPainter ObjPainter { get => _objPainter; set => SetProperty(ref _objPainter, value, nameof(ObjPainter)); }

        public Obj Obj { get => _obj; set => SetProperty(ref _obj, value, nameof(Obj)); }

        public WritableImage Image { get => _image; set => SetProperty(ref _image, value, nameof(Image)); }

        public Color Color { get => _color; set => SetProperty(ref _color, value, nameof(Color)); }

        public ICommand LoadObjCommand { get; }

        public ICommand RepaintCommand { get; }

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

        private void LoadObj()
        {
            var open = FileDialog.ShowDialog();
            if (open != null && open.Value)
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
            if (ObjPainter is null || Obj is null || Image is null)
                return;
            var width = Image.Width;
            var height = Image.Height;
            var viewportMatrix = Matrices.CreateViewportMatrix(0, 0, width, height);
            var projectionMatrix = Matrices.CreateProjectionByAspect((float)width / height, ToRadians(Fov), 0.1f, 100.0f);
            var viewMatrix = Matrices.CreateViewMatrix(CameraPosX, CameraPosY, CameraPosZ, ToRadians(CameraYaw), ToRadians(CameraPitch), ToRadians(CameraRoll));
            var modelMatrix = Matrices.CreateModelMatrix(ModelPosX, ModelPosY, ModelPosZ, ToRadians(ModelYaw), ToRadians(ModelPitch), ToRadians(ModelRoll), ModelScale);

            var model = Obj.Transform(viewportMatrix, projectionMatrix, viewMatrix, modelMatrix);

            ObjPainter.Paint(model, Image, Color);

            NotifyPropertyChanged(nameof(Image));
        }

        private static float ToRadians(float radians)
        {
            return (float)(radians / 180 * Math.PI);
        }
    }
}
