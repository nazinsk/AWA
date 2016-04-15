angular.module('instawa')
.controller('PostCtrl', function ($scope, SEGURANCA, POSTS, $ionicLoading, $ionicActionSheet, $cordovaCamera, $cordovaToast, $ionicScrollDelegate){
  //$scope.descricaoPost = '';
  $scope.postAPostar = {};

  $scope.coletarFoto = function() {
    $ionicActionSheet.show({
     buttons: [
       { text: 'da câmera' },
       { text: 'da biblioteca' }
     ],
     titleText: 'Foto para postar',
     cancelText: 'não quero mais',
     cancel: function() {

     },
     buttonClicked: function(index) {

       var options = {
         quality: 50,
         destinationType: Camera.DestinationType.DATA_URL,
         allowEdit: true,
         encodingType: Camera.EncodingType.JPEG,
         targetWidth: 500,
         targetHeight: 500,
         saveToPhotoAlbum: false,
         correctOrientation:true
       };

      if(index == 0){//Camera
        options.sourceType = Camera.PictureSourceType.CAMERA;
      }
      else if(index == 1){//Biblioteca
        options.sourceType = Camera.PictureSourceType.PHOTOLIBRARY;
      }

      $cordovaCamera.getPicture(options).then(function(imageData) {
        $scope.postAPostar.imagemPost = "data:image/jpeg;base64," + imageData;
      }, function(err) {
        $cordovaToast.showLongBottom('Erro um erro ao coletar a foto.');
      });

      return true;
     }
   });
  }

  $scope.postar = function() {

    if(!$scope.postAPostar.imagemPost){
      $cordovaToast.showLongBottom('ERRO: Tem que ter umo foto para ser postada');
      return;
    }

    POSTS._postar(
        {
          usuarioID: SEGURANCA._dadosUsuarioLogado().ID,
          imagem:$scope.postAPostar.imagemPost,
          descricao: $scope.postAPostar.descricaoPost
        }
      ).then(function (res) {
        //Sucesso

        $scope.postAPostar.imagemPost = undefined;
        $scope.postAPostar.descricaoPost = '';

        $ionicScrollDelegate.scrollTop();

        $cordovaToast.showLongBottom('Postagem realizada com sucesso.');
      },
      function (res) {
        //Erro
        $cordovaToast.showLongBottom('Ocorreu um erro ao postar, tente novamente.');
      });
  }

});
