(function() {
  'use strict';

  angular
    .module('app')
    .controller('Login', Login);

  Users.$inject = ['dataservice', '$state'];
  function Users(dataservice, $state){
    var vm = this;
    vm.userName = "";
    vm.password = "";

    vm.login = login;

    activate();

    function activate() {
    }

    function login() {
      return dataservice.login()
        .then(function () {
          $state.go("Home");
        });
    }

  }
})();
