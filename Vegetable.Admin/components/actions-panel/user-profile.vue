<template>
  <v-list>
    <v-list-group v-model="active">
      <template v-slot:activator>
        <v-list-item-action>
          <v-avatar size="30">
            <img :src="$store.state.owner.user.picture" alt="User" />
          </v-avatar>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{$store.state.owner.user.email}}</v-list-item-title>
        </v-list-item-content>
      </template>

      <v-list-item>
        <v-list-item-action>
          <v-btn text small @click.stop="copyInviteLink">
            <v-icon>file_copy</v-icon>
            <span>{{ $t('user.invite-link') }}</span>
          </v-btn>
        </v-list-item-action>
      </v-list-item>

      <v-list-item>
        <v-list-item-action>
          <v-btn text small @click.stop="logout">
            <v-icon>exit_to_app</v-icon>
            <span>{{ $t('user.logout') }}</span>
          </v-btn>
        </v-list-item-action>
      </v-list-item>
    </v-list-group>
  </v-list>
</template>

<script>
import Constants from "~/config.js";
import Auth0Lock from "auth0-lock";
export default {
  data() {
    return {
      active: true,
      inviteLink:
        Constants.AdminBaseUrl + "login?companyid=" + this.$store.state.owner.id
    };
  },
  methods: {
    logout() {
      this.$store.commit("setAuthenticated", false);      

      // TODO: Need to encapsulate auth0 logic in single module
      var options = {
        auth: {
          params: {
            scope: "openid profile email"
          },
          audience: "vegetable"
        }
      };
      var lock = new Auth0Lock(
        "KpF5kduqFqXVHykbcCDDMYhUI0VPboP3",
        "vegetableproj.eu.auth0.com",
        options
      );

      lock.logout({
        returnTo: Constants.AdminBaseUrl + "login"
      });
     
    },
    copyInviteLink() {
      var text = this.inviteLink;
      var textArea = document.createElement("textArea");
      var range;
      var selection;
      textArea.style.position = "fixed";
      textArea.style.opacity = "0";
      textArea.style.zIndex = "-1";
      textArea.style.right = "0px";
      textArea.style.height = "1px";
      textArea.style.width = "1px";
      textArea.style.pointerEvents = "none";
      textArea.value = text;
      document.body.appendChild(textArea);

      if (navigator.userAgent.match(/ipad|iphone/i)) {
        range = document.createRange();
        range.selectNodeContents(textArea);
        selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
        textArea.setSelectionRange(0, 999999);
      } else {
        textArea.select();
      }

      document.execCommand("copy");
      document.body.removeChild(textArea);
    }
  }
};
</script>

<style scoped>
i.icon {
  cursor: pointer;
}
.list__group:before,
.list__group:after {
  content: none;
}
</style>
