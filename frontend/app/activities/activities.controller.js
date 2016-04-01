(function(){
  'use strict';

  angular
    .module('CRM')
    .controller('Activities', Activities);

  Activities.$inject = ['activities', 'dataservice', '$state', '$stateParams'];
  function Activities(activities, dataservice, $state, $stateParams){
    var vm = this;
    vm.query = {};
    vm.itemCount = 0;
    vm.headers = [
      {
        label: "Name",
        selector: "name"
      },
      {
        label: "Company",
        selector: "companyName"
      },
      {
        label: "Owner",
        selector: "primaryResponsibleName"
      },
      {
        label: "Contact",
        selector: "primaryContactName"
      },
      {
        label: "Due",
        selector: "dueDate",
        format: "date"
      },
      {
        label: "From",
        selector: "dueTimeStart",
        format: "time"
      },
      {
        label: "To",
        selector: "dueTimeEnd",
        format: "time"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "Activities.edit"
      }
    ];

    vm.getActivities = getActivities;

    activate();

    function activate(){
      setupData(activities);
    }

    function getActivities(sortParam){
      _.merge(vm.query, {orderBy: sortParam});
      console.log(vm.query);
      $state.go("Activities.list", vm.query);
    }

    function setupData(data){
      vm.itemCount = data.itemCount;
      _.merge(vm.query, {
        pageSize: data.pageSize,
        page: data.pageNumber,
        orderBy: $stateParams.orderBy
      });

      vm.activities = _.map(data.data, function(a){
        a.dueDate = moment.utc(a.dueDate);
        a.dueDate.local();
        if(a.dueTimeStart){
          a.dueTimeStart = moment.utc(a.dueTimeStart);
          a.dueTimeStart.local();
        }
        if(a.dueTimeEnd){
          a.dueTimeEnd = moment.utc(a.dueTimeEnd);
          a.dueTimeEnd.local();
        }
        return a;
      });
    }
  }
})();
