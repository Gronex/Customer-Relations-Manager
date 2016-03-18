(function(){
  'use strict';

  angular
    .module('CRM')
    .controller('SaveModal', Controller);

  Controller.$inject = ['$uibModalInstance', 'save', 'cancel'];
  function Controller($modal, save, cancel){
    var vm = this;

    vm.save = mySave;
    vm.cancel = myCancel;

    function mySave(){
      if(typeof(save) === "function")
        save(vm.name);
      $modal.close('ok');
    }

    function myCancel(){
      if(typeof(cancel) === "function")
        cancel();
      $modal.close('cancel');
    }
  }
})();
