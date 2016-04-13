(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('OpportunityCategories', OpportunityCategories);

  OpportunityCategories.$inject = ['dataservice', 'warning'];

  /* @ngInject */
  function OpportunityCategories(dataservice, warning) {
    var vm = this;
    vm.opportunityCategories = [];
    vm.opportunityCategory = {};

    vm.create = create;
    vm.save = save;
    vm.remove = remove;

    activate();

    function activate() {
      getOpportunityCategories();
      resetOpportunityCategory();
    }

    function getOpportunityCategories() {
      dataservice.opportunityCategories
        .get()
        .then(function (data) {
          vm.opportunityCategories = _.orderBy(data, ['name']);
        });
    }

    function create() {
      vm.opportunityCategory.errMsg = undefined;
      dataservice.opportunityCategories
        .create(vm.opportunityCategory)
        .then(function (data) {
          vm.opportunityCategories.push(data.data);
          resetOpportunityCategory();
        }, function (err) { handleError(err, vm.opportunityCategory); });
    }
    function save(opportunityCategory) {
      opportunityCategory.errMsg = undefined;
      if(opportunityCategory.saved || opportunityCategory.saved == undefined) return;
      dataservice.opportunityCategories
        .update(opportunityCategory.id, opportunityCategory)
        .then(function () {
          opportunityCategory.saved = true;
          resetOpportunityCategory();
        }, function (err) { handleError(err, opportunityCategory); });
    }
    function remove(id) {
      warning.warn({text: ["This category may be connected to other things, and deleting it will remove it from these as well.", "Are you sure you want to continue?"]})
        .then(function(){
          dataservice.opportunityCategories
            .remove(id)
            .then(function () {
              _.remove(vm.opportunityCategories, function (s) {
                return s.id == id;
              });
            });
        });
    }

    function resetOpportunityCategory() {
      vm.opportunityCategories = _.orderBy(vm.opportunityCategories, ['name']);
      vm.opportunityCategory = {
        name: ""
      };
    }

    function handleError(err, target) {
      switch (err.status) {
        case 400:

          break;
        case 409:
          target.errMsg = "Group with name: '" + err.data.name + "' already exists";
          break;
        default:
      }
    }
  }
})();
