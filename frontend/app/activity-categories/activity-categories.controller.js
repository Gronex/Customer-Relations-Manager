(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('ActivityCategories', ActivityCategories);

  ActivityCategories.$inject = ['dataservice', 'warning'];

  /* @ngInject */
  function ActivityCategories(dataservice, warning) {
    var vm = this;
    vm.activityCategories = [];
    vm.activityCategory = {};

    vm.create = create;
    vm.save = save;
    vm.remove = remove;

    activate();

    function activate() {
      getActivityCategories();
      resetActivityCategory();
    }

    function getActivityCategories() {
      dataservice.activityCategories
        .get()
        .then(function (data) {
          vm.activityCategories = data;
        });
    }

    function create() {
      vm.activityCategory.errMsg = undefined;
      dataservice.activityCategories
        .create(vm.activityCategory)
        .then(function (data) {
          vm.activityCategories.push(data.data);
          resetActivityCategory();
        }, function (err) { handleError(err, vm.activityCategory); });
    }

    function save(activityCategory) {
      activityCategory.errMsg = undefined;
      if(activityCategory.saved || activityCategory.saved == undefined) return;
      dataservice.activityCategories
        .update(activityCategory.id, activityCategory)
        .then(function () {
          activityCategory.saved = true;
          resetActivityCategory();
        }, function (err) { handleError(err, activityCategory); });
    }

    function remove(id) {
      warning.warn({text: ["This category may be connected to other things, and deleting it will remove it from these as well.", "Are you sure you want to continue?"]}).then(function(){
        dataservice.activityCategories
          .remove(id)
          .then(function () {
            _.remove(vm.activityCategories, function (s) {
              return s.id == id;
            });
          });
      });
    }

    function resetActivityCategory() {
      vm.activityCategories = _.orderBy(vm.activityCategories, ['value']);
      vm.activityCategory = {
        value: 0,
        name: ""
      };
    }

    function handleError(err, target) {
      switch (err.status) {
        case 400:

          break;
        case 409:
          target.errMsg = "ActivityCategory with name: '" + err.data.name + "' already exists";
          break;
        default:
      }
    }
  }
})();
