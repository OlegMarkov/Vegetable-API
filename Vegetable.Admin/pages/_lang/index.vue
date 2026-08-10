<template>
  <v-app>
    <v-navigation-drawer fixed hide-overlay clipped app v-model="drawer">
      <actions-panel-view></actions-panel-view>
      <v-footer app fixed class="pa-4">
        <v-spacer></v-spacer>
        <div>&copy; {{ new Date().getFullYear() }}</div>
      </v-footer>
    </v-navigation-drawer>
    <v-app-bar app fixed dense clipped-left>
      <v-app-bar-nav-icon @click.native="drawer = !drawer"></v-app-bar-nav-icon>
      <span class="title ml-4 mr-12">{{ $t('main.title') }}</span>
      <v-spacer></v-spacer>
      </v-app-bar>
    <v-content>
      <v-container container--fluid>
        <calendar-view></calendar-view>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
// import { normalize, schema, denormalize } from 'normalizr'
import axios from "axios";
import _ from "underscore";
import CalendarView from "~/components/calendar.vue";
import CalendarHeaderView from "~/components/calendar/calendar-header.vue";
import ActionsPanelView from "~/components/actions-panel/actions-panel.vue";
import Constants from "~/config.js";

import {
    SET_SERVICES,
    SET_EMPLOYEES
} from "@/store/mutations.type";

export default {
  data: context => {
    return {
      selectedCalendarType: "MONTH",
      currentDate: {},
      drawer: null
    };
  },

  created() {
    let me = this;
    this.$root.$on("CHANGE_DATE", function(payload) {
      me.currentDate = payload;
    });
    this.$root.$on("CHANGE_CALENDAR_TYPE", function(value) {
      me.selectedCalendarType = value;
    });
    this.$root.$on("CHANGE_SCHEDULE", function(value) {
      me.currentDate = value;
      me.schedule_dialog = true;
    });
  },

  fetch: context => {
    return axios
      .get(Constants.ApiOwnerUrl)
      .then(response => {
        var owner = response.data;
        var selectedEmployees = [];
        var selectedServices = [];

        var color = [
          "red",
          "pink",
          "purple",
          "deep-purple",
          "indigo",
          "blue",
          "light-blue",
          "cyan",
          "teal",
          "green",
          "light-green",
          "lime",
          "yellow",
          "amber",
          "orange",
          "deep-orange",
          "blue-grey",
          "grey"
        ];
        var colorIndex = 0;

        _.each(owner.employees, function(employee) {
          selectedEmployees.push({
            id: employee.id,
            checked: true,
            color: color[colorIndex]
          });
          colorIndex++;
        });

        _.each(owner.services, function(service) {
          selectedServices.push({
            id: service.id,
            checked: true,
            color: color[colorIndex]
          });
          colorIndex++;
        });

        context.store.commit("setOwner", {...owner});
        context.store.commit(SET_SERVICES, owner.services);
        context.store.commit(SET_EMPLOYEES, owner.employees);
      })
      .catch(error => {
        console.log(error.response);
        context.store.commit("setOwner", {});
      });
  },

  components: {
    CalendarView,
    CalendarHeaderView,
    ActionsPanelView
  },
  computed: {},
  methods: {}
};
</script>

<style scoped>
::-webkit-scrollbar {
  width: 6px;
}

/* Track */
::-webkit-scrollbar-track {
  background: #f1f1f1;
}

/* Handle */
::-webkit-scrollbar-thumb {
  background: #b0bec5;
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
  background: #cfd8dc;
}
</style>
