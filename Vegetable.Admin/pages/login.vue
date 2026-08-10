<style>
.container {
  min-height: 20vh;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
}

.title {
  font-family: "Quicksand", "Source Sans Pro", -apple-system, BlinkMacSystemFont,
    "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
  /* 1 */
  display: block;
  font-weight: 300;
  font-size: 100px;
  color: #35495e;
  letter-spacing: 1px;
}

.subtitle {
  font-weight: 300;
  font-size: 42px;
  color: #526488;
  word-spacing: 5px;
  padding-bottom: 15px;
}
</style>

<template>
  <div>
    <section class="container">
      <div>
        <h1 class="title">{{ $t('login.title') }}</h1>
        <h2 class="subtitle">{{ $t('login.subtitle') }}</h2>
      </div>
    </section>
    <div id="login-container"></div>
  </div>
</template>

<script>
import Auth0Lock from "auth0-lock";
import axios from "axios";
import Constants from "~/config.js";

export default {
  mounted() {
    var urlParams = new URLSearchParams(window.location.search);
    var companyId = urlParams.get("companyid");

    var options = {
      autoclose: true,
      closable: false,
      container: "login-container",
      auth: {
        params: {
          scope: "openid profile email"
        },
        audience: "vegetable"
      }
    };    

    if (companyId) {
      this.$store.commit("setTempCompanyId", companyId);
      options.allowLogin = false;
    }

     if (!this.$store.state.owner.authenticated) {
      this.$store.commit("setUser", {});
      this.$store.commit("setOwner", {});      
    }

    var lock = new Auth0Lock(
      "KpF5kduqFqXVHykbcCDDMYhUI0VPboP3",
      "vegetableproj.eu.auth0.com",
      options
    );
   

    if (!this.$store.state.owner.authenticated || companyId) {      
      lock.show();
    }

    lock.on(
      "authenticated",
      function(authResult) {
        this.$store.commit("setAuthenticated", true);
        lock.hide();

        lock.getUserInfo(
          authResult.accessToken,
          function(error, profile) {
            if (!error) {
              this.$store.commit("setUser", profile);

              // create new owner
              if (
                !this.$store.state.owner.tempCompanyId &&
                !profile[Constants.OwnerIdField]
              ) {
                var owner = {
                  userId: profile.sub
                };
                axios.post(Constants.ApiOwnerUrl, owner).then(response => {
                  this.$store.commit("setTempCompanyId", null);
                  // get new token with user_metadata
                  lock.checkSession(
                    {},
                    function(err, authResult) {
                      this.$store.commit("setAuthenticated", true);
                      window.location.replace("/");
                    }.bind(this)
                  );
                });
                // create new user for existing owner
              } else if (!profile[Constants.OwnerIdField]) {
                var ownerMeta = {
                  userId: profile.sub,
                  id: this.$store.state.owner.tempCompanyId
                };
                axios
                  .post(Constants.ApiUserUrl + "updatemetadata", ownerMeta)
                  .then(response => {
                    this.$store.commit(
                      "setTempCompanyId",
                      this.$store.state.owner.tempCompanyId
                    );
                    window.location.replace("/");
                  });
                // login with existing user
              } else {
                this.$store.commit("setTempCompanyId", null);
                window.location.replace("/");
              }
            }
          }.bind(this)
        );
      }.bind(this)
    );
  }
};
</script>