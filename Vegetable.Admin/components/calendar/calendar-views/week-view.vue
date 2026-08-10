<template>
  <v-container container--fluid>
    <v-row>
      <v-col sm="1" mt-2>
        <v-container container--fluid>
          <v-card height="3em" ml-4 flat></v-card>
        </v-container>
        <v-col v-for="(timeOffset, index) in dayOffsets" :key="index" offset-sm="6">
          <v-card height="1.5em" width="50px" ml-4 flat>
            <v-col v-if="timeOffset.IsShow" align-center pl-2>{{timeOffset.DisplayTime}}</v-col>
          </v-card>
        </v-col>
      </v-col>
      <v-row mb-4>
        <v-col v-for="(day, index) in currentWeekDays" :key="index">
          <day :daydate="day"></day>
        </v-col>
      </v-row>
    </v-row>
  </v-container>
</template>
<script>
import Day from "~/components/calendar/day.vue";
let moment = require("moment");
export default {
  props: {
    currentDate: {}
  },

  components: {
    Day
  },

  computed: {
    dayOffsets() {
      var now = new Date();
      var hoursPeriods = ["00", "30"];
      var times = [];
      for (var i = 0; i < 24; i++) {
        for (var j = 0; j < 2; j++) {
          var time = i + ":" + hoursPeriods[j];
          var isShow = j !== 1;
          now.setHours(i);
          now.setMinutes(hoursPeriods[j]);
          times.push({
            Time: time,
            IsShow: isShow,
            DisplayTime: now.toLocaleTimeString([], {
              hour: "2-digit",
              minute: "2-digit"
            })
          });
        }
      }
      return times;
    },
    currentWeekDays() {
      let startOfWeek = moment(this.currentDate).startOf("isoWeek");
      let endOfWeek = moment(this.currentDate).endOf("isoWeek");

      let days = [];
      let day = startOfWeek;

      while (day <= endOfWeek) {
        days.push(day.locale(this.$store.state.owner.locale));
        day = day.clone().add(1, "d");
      }

      return days;
    }
  },

  methods: {},

  filters: {
    weekDayName(day) {
      return day.localeData().weekdays()[day.day()];
    }
  }
};
</script>