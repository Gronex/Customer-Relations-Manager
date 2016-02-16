(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('App', App);

  App.$inject = ['authorization'];
  function App(authorization){
    var vm = this;

    vm.user;
    vm.logout = logout;

    activate();

    function activate() {
      if(authorization.configToken()){
        vm.user = authorization.getUser();
      }
    }

    function logout() {
      authorization.logout();
      vm.user = undefined;
    }

  }
})();
