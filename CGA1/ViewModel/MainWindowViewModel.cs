using CGA1.Command;
using CGA1.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System;
using System.Numerics;

namespace CGA1.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private Obj _obj;
        private Image _image;

        public MainWindowViewModel()
        {
            FileDialog = new OpenFileDialog
            {
                Filter = "Assembly | *.dll",
                Title = "Select assembly",
                Multiselect = false
            };
            LoadObjCommand = new DelegateCommand(o =>
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
            });
            RepaintCommand= new DelegateCommand(o =>
            {
                var width = ImageWidth;
                var height = ImageHeight;
                var image = new WritableImage(width, height);
                var viewportMatrix = Matrices.CreateViewportMatrix(0, 0, width, height);
                var projectionMatrix = Matrices.CreateProjectionByAspect((float)width / height, ToRadians(60), 0.1f, 100.0f);

                var viewMatrix = Matrices.CreateViewMatrix(CameraPosition, CameraRotation);
                var modelMatrix = Matrices.CreateModelMatrix(ModelPosition, ModelRotation, ModelScale);
                var model = Obj.Transform(viewportMatrix, projectionMatrix, viewMatrix, modelMatrix);

                ObjPainter.Paint(model, image, Color);

                Image.Source = image.Source;

                NotifyPropertyChanged(nameof(Image));
            });
        }

        public int ImageWidth
        {
            get
            {
                return (int)Image.ActualWidth;
            }
        }

        public int ImageHeight
        {
            get
            {
                return (int)Image.ActualHeight;
            }
        }

        public Vector3 ModelPosition
        {
            get
            {
                var x = (float)(ModelPositionXSlider.Value);
                var y = (float)(ModelPositionYSlider.Value);
                var z = (float)(ModelPositionZSlider.Value);
                return new Vector3(x, y, z);
            }
        }

        public Vector3 ModelRotation
        {
            get
            {
                var yaw = (float)(ModelYawSlider.Value * Math.PI / 180);
                var pitch = (float)(ModelPitchSlider.Value * Math.PI / 180);
                var roll = (float)(ModelRollSlider.Value * Math.PI / 180);
                return new Vector3(yaw, pitch, roll);
            }
        }

        private float ModelScale
        {
            get
            {
                return (float)(ModelScaleSlider.Value);
            }
        }

        public Vector3 CameraPosition
        {
            get
            {
                var x = (float)(CameraPositionXSlider.Value);
                var y = (float)(CameraPositionYSlider.Value);
                var z = (float)(CameraPositionZSlider.Value);
                return new Vector3(x, y, z);
            }
        }

        public Vector3 CameraRotation
        {
            get
            {
                var yaw = (float)(CameraYawSlider.Value * Math.PI / 180);
                var pitch = (float)(CameraPitchSlider.Value * Math.PI / 180);
                var roll = (float)(CameraRollSlider.Value * Math.PI / 180);
                return new Vector3(yaw, pitch, roll);
            }
        }

        public Color Color
        {
            get
            {
                return default;
            }
        }

        public IObjPainter ObjPainter
        {
            get
            {
                return new BresenhamPainter();
            }
        }

        private FileDialog FileDialog { get; }

        public Obj Obj { get => _obj; set => SetProperty(ref _obj, value); }

        public Image Image { get => _image; set => SetProperty(ref _image, value); }

        public ICommand LoadObjCommand { get; }

        public ICommand RepaintCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
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

        private static float ToRadians(float radians)
        {
            return (float)(radians * Math.PI / 180);
        }
    }
}
