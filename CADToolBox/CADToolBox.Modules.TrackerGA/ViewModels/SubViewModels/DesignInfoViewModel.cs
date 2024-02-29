using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Media;
using System;
using System.Windows;
using CADToolBox.Shared.Models.CADModels.Implement;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;

public partial class DesignInfoViewModel : ViewModelBase {
    [ObservableProperty]
    private TrackerModel? _trackerModel;


    public DesignInfoViewModel() {
        TrackerModel = TrackerApp.Current.TrackerModel;

        Draw();

        TrackerModel!.PropertyChanged += OnTrackerModelChanged;
    }

#region 绘图用临时属性与方法

    [ObservableProperty]
    private double _canvasHeight = 500;

    [ObservableProperty]
    private double _canvasWidth = 700;

    [ObservableProperty]
    private PointCollection? _pilePoints;

    [ObservableProperty]
    private PointCollection? _postPoints;

    [ObservableProperty]
    private PointCollection? _beamPoints;

    [ObservableProperty]
    private PointCollection? _purlinPoints;

    [ObservableProperty]
    private PointCollection? _modulePoints;

    private void Draw() {
        var moduleLength     = TrackerModel!.ModuleLength;
        var moduleWidth      = TrackerModel!.ModuleWidth;
        var moduleHeight     = TrackerModel!.ModuleHeight;
        var moduleGapChord   = TrackerModel!.ModuleGapChord;
        var purlinHeight     = TrackerModel!.PurlinHeight;
        var purlinLength     = TrackerModel!.PurlinLength;
        var beamHeight       = TrackerModel!.BeamHeight;
        var beamWidth        = TrackerModel!.BeamWidth;
        var moduleRowCounter = TrackerModel!.ModuleRowCounter;

        var minGroundDist = TrackerModel!.MinGroundDist;
        var stowAngle     = TrackerModel!.StowAngle;
        var maxAngle      = TrackerModel!.MaxAngle;

        var beamCenterToDrivePost   = TrackerModel!.BeamCenterToDrivePost;
        var beamCenterToGeneralPost = TrackerModel!.BeamCenterToGeneralPost;
        var beamRadio               = TrackerModel!.BeamRadio;
        var pileUpGround            = TrackerModel!.PileUpGround;
        var pileWidth               = TrackerModel!.PileWidth;
        var postWidth               = TrackerModel!.PostWidth;

        var chord = moduleRowCounter < 2
                        ? moduleLength
                        : moduleLength * moduleRowCounter + (moduleRowCounter - 1) * moduleGapChord;

        stowAngle *= Math.PI / 180;
        maxAngle  *= Math.PI / 180;

        var beamHeightUp = beamHeight * beamRadio / (beamRadio + 1);
        var beamCenterToGround = minGroundDist + chord / 2 * Math.Sin(maxAngle) -
                                 (beamHeightUp + purlinHeight) * Math.Cos(maxAngle);

        var maxHeight = minGroundDist + chord * Math.Sin(maxAngle) + moduleHeight * Math.Cos(maxAngle);

        var maxWidth = (chord / 2 * Math.Cos(maxAngle) + (beamHeightUp + purlinHeight) * Math.Sin(maxAngle)) * 2;

        const double canvasRadio = 0.1; // 全局绘图比例


        pileWidth               *= canvasRadio;
        pileUpGround            *= canvasRadio;
        postWidth               *= canvasRadio;
        chord                   *= canvasRadio;
        beamCenterToGround      *= canvasRadio;
        beamCenterToDrivePost   *= canvasRadio;
        beamCenterToGeneralPost *= canvasRadio;

        var postHeight = beamCenterToGround - Math.Min(beamCenterToDrivePost, beamCenterToGeneralPost) - pileUpGround;

        beamHeight   *= canvasRadio;
        beamWidth    *= canvasRadio;
        beamHeightUp *= canvasRadio;
        purlinHeight *= canvasRadio;
        purlinLength *= canvasRadio;
        moduleHeight *= canvasRadio;

        var beamHeightDown = beamHeight - beamHeightUp;
        var theta1         = Math.Atan(beamHeightUp / beamWidth * 2);
        var theta2         = Math.PI - 2 * theta1;
        var theta3         = Math.Atan(beamHeightDown / beamWidth * 2);
        var theta4         = Math.PI - 2 * theta3;
        var beamR1         = beamHeightUp   / Math.Sin(theta1);
        var beamR2         = beamHeightDown / Math.Sin(theta3);


        var centerX = CanvasWidth / 2;
        var centerY = CanvasHeight;
        PilePoints = [
                         new Point(centerX - pileWidth / 2, centerY),
                         new Point(centerX - pileWidth / 2, centerY - pileUpGround),
                         new Point(centerX + pileWidth / 2, centerY - pileUpGround),
                         new Point(centerX + pileWidth / 2, centerY)
                     ];

        centerY -= pileUpGround;
        PostPoints = [
                         new Point(centerX - postWidth / 2, centerY),
                         new Point(centerX - postWidth / 2, centerY - postHeight),
                         new Point(centerX + postWidth / 2, centerY - postHeight),
                         new Point(centerX + postWidth / 2, centerY)
                     ];

        centerY -= postHeight + Math.Min(beamCenterToDrivePost, beamCenterToGeneralPost);
        BeamPoints = [
                         new Point(centerX + beamR1 * Math.Cos(theta1 + maxAngle),
                                   centerY - beamR1 * Math.Sin(theta1 + maxAngle)),
                         new Point(centerX + beamR1 * Math.Cos(theta1 + theta2 + maxAngle),
                                   centerY - beamR1 * Math.Sin(theta1 + theta2 + maxAngle)),
                         new Point(centerX + beamR2 * Math.Cos(Math.PI + theta3 + maxAngle),
                                   centerY - beamR2 * Math.Sin(Math.PI + theta3 + maxAngle)),
                         new Point(centerX + beamR2 * Math.Cos(Math.PI + theta3 + theta4 + maxAngle),
                                   centerY - beamR2 * Math.Sin(Math.PI + theta3 + theta4 + maxAngle))
                     ];

        centerX -= (beamHeightUp + purlinHeight / 2) * Math.Sin(maxAngle);
        centerY -= (beamHeightUp + purlinHeight / 2) * Math.Cos(maxAngle);
        var purlinR      = Math.Sqrt(purlinLength * purlinLength + purlinHeight * purlinHeight) / 2;
        var purlinTheta1 = Math.Atan(purlinHeight / purlinLength);
        var purlinTheta2 = Math.PI - 2 * purlinTheta1;
        PurlinPoints = [
                           new Point(centerX + purlinR * Math.Cos(purlinTheta1 + maxAngle),
                                     centerY - purlinR * Math.Sin(purlinTheta1 + maxAngle)),
                           new Point(centerX + purlinR * Math.Cos(purlinTheta1 + purlinTheta2 + maxAngle),
                                     centerY - purlinR * Math.Sin(purlinTheta1 + purlinTheta2 + maxAngle)),
                           new Point(centerX + purlinR * Math.Cos(Math.PI + purlinTheta1 + maxAngle),
                                     centerY - purlinR * Math.Sin(Math.PI + purlinTheta1 + maxAngle)),
                           new Point(centerX + purlinR * Math.Cos(Math.PI + purlinTheta1 + purlinTheta2 + maxAngle),
                                     centerY - purlinR * Math.Sin(Math.PI + purlinTheta1 + purlinTheta2 + maxAngle))
                       ];

        centerX -= (moduleHeight + purlinHeight) / 2 * Math.Sin(maxAngle);
        centerY -= (moduleHeight + purlinHeight) / 2 * Math.Cos(maxAngle);
        var moduleR      = Math.Sqrt(chord * chord + moduleHeight * moduleHeight) / 2;
        var moduleTheta1 = Math.Atan(moduleHeight / chord);
        var moduleTheta2 = Math.PI - 2 * moduleTheta1;
        ModulePoints = [
                           new Point(centerX + moduleR * Math.Cos(moduleTheta1 + maxAngle),
                                     centerY - moduleR * Math.Sin(moduleTheta1 + maxAngle)),
                           new Point(centerX + moduleR * Math.Cos(moduleTheta1 + moduleTheta2 + maxAngle),
                                     centerY - moduleR * Math.Sin(moduleTheta1 + moduleTheta2 + maxAngle)),
                           new Point(centerX + moduleR * Math.Cos(Math.PI + moduleTheta1 + maxAngle),
                                     centerY - moduleR * Math.Sin(Math.PI + moduleTheta1 + maxAngle)),
                           new Point(centerX + moduleR * Math.Cos(Math.PI + moduleTheta1 + moduleTheta2 + maxAngle),
                                     centerY - moduleR * Math.Sin(Math.PI + moduleTheta1 + moduleTheta2 + maxAngle))
                       ];
    }

#endregion

    private void OnTrackerModelChanged(object    sender,
                                       EventArgs e) {
        Draw();
        //MessageBox.Show("模型发生改变");
    }
}