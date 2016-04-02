(function() {
  'use strict';

  angular
    .module('starter')
    .config(routeConfig);

  function routeConfig($stateProvider, $urlRouterProvider, $ionicConfigProvider) {

    $urlRouterProvider.otherwise('/t1')

    $stateProvider
      .state('T1', {
        url: '/t1',
        templateUrl: 'views/tela1.html',
        controller: 'Tela1'
      })
      .state('T2', {
        url: '/t2',
        templateUrl: 'views/tela2.html',
        controller: 'Tela2'
      });

  }
})();
