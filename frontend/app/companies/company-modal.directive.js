(function(){
  'use strive';

  angular
    .module('CRM')
    .directive('crmCompanyModal', CompanyModal);

  CompanyModal.$inject = ['$uibModal'];
  function CompanyModal($modal){
    
    var directive = {
      link: link,
      restrict: 'AE',
      scope: {
        onSave: "="
      }
    };

    function link(scope, ele, ettr){
      ele.on('click', function(){
        $modal.open({
          templateUrl: "view/app/companies/company-modal.html",
          controllerAs: 'vm',
          controller: modalController,
          resolve: {
            onSave: function(){ return scope.onSave; }
          }
        });
      });
    }

    modalController.$inject = ['$uibModalInstance', 'dataservice', 'onSave'];
    function modalController($modalInstance, dataservice, onSave){
      var vm = this;

      vm.save = save;
      vm.cancel = cancel;

      function save(){
        dataservice.companies
          .create(vm.company)
          .then(function(result){
            var company = result.data;
            company.id = result.location;
            onSave(company);
            $modalInstance.close("ok");
        });
      }

      function cancel(){
        $modalInstance.close("cancel");
      }
    }

    return directive;
  }
})();
