var normalCam : Camera;
var inverseCam : Camera;
var wipeTime = 2.0;
var rotateAmount = 0.0;
var transitionMesh : Mesh;
var shapeScale = 1.5;


function InverseWorld (isInverseWorld : boolean) {
    Screenwipe.ModelScale = shapeScale;
    yield Screenwipe.use.ShapeWipe(         isInverseWorld ? normalCam : inverseCam,
                                            isInverseWorld ? inverseCam : normalCam,
                                            wipeTime,
                                            ZoomType.Grow,
                                            transitionMesh,
                                            rotateAmount);
}
