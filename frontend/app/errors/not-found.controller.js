(function(){
  'use strict';

  angular
    .module("CRM")
    .controller("NotFound", Controller);

  Controller.$inject = [];
  function Controller(){
    var vm = this;

    activate();

    function activate(){
      vm.error = "Not found";
      vm.message = "The page you tried to access does not exist.";
    }
  }
})();
