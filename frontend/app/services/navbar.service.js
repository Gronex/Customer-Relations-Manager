(function(){
  angular
    .module('CRM')
    .factory('navbar', Navbar);

  Navbar.$inject = ['authorization'];
  function Navbar(auth) {
    return {
      generate: generate
    };

    function generate(){
      var user = auth.getUser();

      var navbar = {
        label: "CRM",
        link: "Home",
        left: [],
        right: []
      };

      if(hasRole(user, "Standard")){
        navbar.left = navbar.left.concat([
          {
            link: "Opportunities",
            label: "Opportunities"
          },
          {
            link: "Companies",
            label: "Companies"
          },
          {
            link: "People",
            label: "People"
          },
          {
            link: "Activities",
            label: "Activities"
          }
        ]);
      }

      if(hasRole(user, "Super")){
        navbar.right = navbar.right.concat([
          {
            type: "dropdown",
            label: "Administrate System",
            contents: [
              {
                label: "Users",
                link: "Users"
              },
              {
                label: "Groups",
                link: "UserGroups"
              },
              {
                label: "Departments",
                link: "Departments"
              },
              {
                label: "Stages",
                link: "Stages"
              },
              {
                label: "Activity Categories",
                link: "ActivityCategories"
              },
              {
                label: "OpportunityCategories",
                link: "OpportunityCategories"
              }
            ]
          }
        ]);
      }

      if(user){
        navbar.right.push({
          type: "dropdown",
          label: user.name,
          contents: [
            {
              label: "Log out",
              link: "Logout"
            }
          ]
        })
      } else {
        navbar.right.push({
          label: "Log in",
          link: "Login"
        });
      }

      return navbar;
    }

    function hasRole(user, role){
      if(!user) return false;
      return _.includes(user.roles, role);
    }

  }
})();
