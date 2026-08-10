<template>
  <v-container>
    <v-row>
      <v-spacer></v-spacer>      
      <v-col cols="1">
        <v-btn text icon color="indigo" @click.stop="goPrev">
          <v-icon>keyboard_arrow_left</v-icon>
        </v-btn>
      </v-col>
      <v-col cols="3">
        <v-menu
          ref="menu"
          :close-on-content-click="false"
          v-model="menu"
          transition="scale-transition"
          offset-y
          :nudge-right="40"
          min-width="150px"
          :return-value.sync="date"
        >
          <template v-slot:activator="{ on }">
            <v-text-field v-on="on" v-model="displayDate" readonly></v-text-field>
          </template>
          <v-date-picker
            v-model="date"
            no-title
            scrollable
            @change="selectDate"
            :type="calendarView"
          >
            <v-spacer></v-spacer>
            <v-btn text color="primary" @click="menu = false">Cancel</v-btn>
            <v-btn text color="primary" @click="$refs.menu.save(date)">OK</v-btn>
          </v-date-picker>
        </v-menu>
      </v-col>
      <v-col cols="1"> 
        <v-btn text icon color="indigo" @click.stop="goNext">
          <v-icon>keyboard_arrow_right</v-icon>
        </v-btn>
      </v-col>
      <v-spacer></v-spacer>
      <v-col cols="3">
        <v-select
          :items="calendarTypes"
          v-model="selectedCalendarType"
          @change="onCalendTypeChanged"
          item-text="display"
          item-value="value"
          return-object
        ></v-select>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
let moment = require("moment");

export default {
  data(context) {
    return {
      date: null,
      menu: false,
      modal: false,
      calendarTypes: [],
      selectedCalendarType: {}
    };
  },

  props: {
    currentDate: {}
  },

  computed: {
    displayDate() {
      return moment(this.currentDate).format("MMMM, YYYY");
    },

    calendarView() {
      if (this.selectedCalendarType.value === "MONTH") {
        return "month";
      } else {
        return "date";
      }
    }
  },

  created() {
    this.calendarTypes = [
      {
        display: this.$t("calendar.type-month"),
        value: "MONTH"
      },
      {
        display: this.$t("calendar.type-week"),
        value: "WEEK"
      },
      {
        display: this.$t("calendar.type-day"),
        value: "DAY"
      }
    ];

    this.selectedCalendarType = this.calendarTypes[0];
  },

  methods: {
    goPrev() {
      let payload = moment(this.currentDate)
        .subtract(1, "months")
        .startOf("month");
      if (this.selectedCalendarType.value === "DAY") {
        payload = moment(this.currentDate).subtract(1, "days");
      } else if (this.selectedCalendarType.value === "WEEK") {
        payload = moment(this.currentDate).subtract(1, "weeks");
      }

      this.$root.$emit("CHANGE_DATE", payload);
    },

    goNext() {
      let payload = moment(this.currentDate)
        .add(1, "months")
        .startOf("month");
      if (this.selectedCalendarType.value === "DAY") {
        payload = moment(this.currentDate).add(1, "days");
      } else if (this.selectedCalendarType.value === "WEEK") {
        payload = moment(this.currentDate).add(1, "weeks");
      }

      this.$root.$emit("CHANGE_DATE", payload);
    },

    goToday() {
      this.$root.$emit("CHANGE_DATE", moment());
    },

    selectDate() {
      this.$root.$emit("CHANGE_DATE", moment(this.date));
    },

    localizeCurrentDate() {
      return this.currentDate;
    },

    onCalendTypeChanged(event) {
      this.$root.$emit("CHANGE_CALENDAR_TYPE", event.value);
    }
  }
};
</script>