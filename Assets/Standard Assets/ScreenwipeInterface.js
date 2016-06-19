var normalCam : Camera;
var inverseCam : Camera;
var wipeTime = 2.0;
var rotateAmount = 0.0;
var transitionMesh : Mesh;



function InverseWorld (isInverseWorld : boolean) {
    yield Screenwipe.use.ShapeWipe(         isInverseWorld ? normalCam : inverseCam,
                                            isInverseWorld ? inverseCam : normalCam,
                                            wipeTime,
                                            ZoomType.Grow,
                                            transitionMesh,
                                            rotateAmount);
}
