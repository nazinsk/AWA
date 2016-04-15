angular.module('instawa')
.controller('PerfilCtrl', function ($scope, $rootScope, $state, SEGURANCA, USUARIOS, POSTS, $ionicLoading, $ionicActionSheet, $cordovaCamera, $ionicPopup){

  $scope.usuarioLogado = SEGURANCA._dadosUsuarioLogado();
  $scope.usuarioPosts = [];
  atualizaFeed();

  //Evento Seleciona Tab
  $rootScope.$on('tab2', function(){
    $scope.usuarioLogado = SEGURANCA._dadosUsuarioLogado();
    atualizaFeed();
  });

  //Evento Seleciona Login
  $rootScope.$on('login', function(){
    $scope.usuarioLogado = SEGURANCA._dadosUsuarioLogado();
    atualizaFeed();
  });

  $scope.sair = function() {
    SEGURANCA._efetuarLogoff();
    $state.go('login');
  }

  $scope.alterarDados = function() {
    $ionicPopup.show({
      templateUrl: 'views/alteraDados.html',
      title: 'Alterar seus dados',
      scope: $scope,
      buttons: [
        { text: 'Cancelar' },
        {
          text: '<b>Alterar</b>',
          type: 'button-positive',
          onTap: function(e) {
            USUARIOS._salva($scope.usuarioLogado)
            .then(
              function(res) {
                $scope.usuarioLogado.senha = '';
                //Atualiza dados do usuário logado
                SEGURANCA._gravaLogin(res.data);
              }
            );
          }
        }
      ]
    });

  }

  $scope.excluir = function(post) {
    $ionicPopup.confirm({
     title: 'Excluir Post',
     template: 'Deseja excluir o post? <br/>Não poderá ser desfeito.'
   }).then(function(res) {
     if(res) {
       //SIM

       $ionicLoading.show();

       POSTS._excluir(post.ID)
       .then(
         function(res) {
           $scope.usuarioPosts.splice($scope.usuarioPosts.indexOf(post), 1);
           $ionicLoading.hide();
           $scope.$broadcast('scroll.refreshComplete');
           $cordovaToast.showLongBottom('Post removido.');
         }, function(err) {
           $cordovaToast.showLongBottom('Erro um erro ao excluir o post.');
         });

     } else {
       //NÃO
     }
   });
  }



  $scope.trocarFoto = function() {

    $ionicActionSheet.show({
     buttons: [
       { text: 'da câmera' },
       { text: 'da biblioteca' }
     ],
     titleText: 'Trocar sua foto',
     cancelText: 'não trocar mais',
     cancel: function() {

     },
     buttonClicked: function(index) {

       var options = {
         quality: 50,
         destinationType: Camera.DestinationType.DATA_URL,
         allowEdit: true,
         encodingType: Camera.EncodingType.JPEG,
         targetWidth: 100,
         targetHeight: 100,
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
        $scope.usuarioLogado.foto = "data:image/jpeg;base64," + imageData;

        //Varre os posts e atualiza as fotos
        angular.forEach($scope.usuarioPosts, function(value, key) {
          value.usuarioDados.foto = $scope.usuarioLogado.foto;
        });

        USUARIOS._salva($scope.usuarioLogado)
        .then(
          function(res) {
            //Atualiza dados do usuário logado
            SEGURANCA._gravaLogin(res.data);
          }
        );
      }, function(err) {
        $cordovaToast.showLongBottom('Erro um erro ao coletar a foto.');
      });

      return true;
     }
   });
  }



  function atualizaFeed() {

    $ionicLoading.show();

    POSTS._listarUsuario($scope.usuarioLogado.ID)
    .then(
      function(res) {
        $scope.usuarioPosts = res.data;
        $ionicLoading.hide();
        $scope.$broadcast('scroll.refreshComplete');
      },
      function(res) {
        $cordovaToast.showLongBottom('Erro ao buscar as postagens do usuário.');
        $ionicLoading.hide();
        $scope.$broadcast('scroll.refreshComplete');
      }
    );
  }

  $scope.atualizar = atualizaFeed;
});
