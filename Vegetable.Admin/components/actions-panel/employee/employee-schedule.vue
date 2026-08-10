<template>
<v-container container--fluid>
  <v-row wrap>
<!--     <v-row>
      <v-spacer></v-spacer>
      <v-col xs12 sm6 md4>{{ workTimeTitle }}</v-col>
      <v-spacer></v-spacer>
      <v-col xs12 sm6 md4>{{ breakTimeTitle }}</v-col>
    </v-row> -->
    <v-col sm3>
            <v-text-field type="number" name="repeat-input" label="Repeat every"
            id="repeatNumber" min=0 max=500 step=1 maxlength=3 v-model="every"
          ></v-text-field>
        </v-col>
        <v-col sm3>
            <v-select :items="periods" item-value="value" item-text="dn" v-model="selectedPeriod" label="Select" single-line></v-select>
        </v-col>
        <v-col sm6>
        </v-col>
        <v-row wrap v-if="showDaysSch">
    <day-schedule
      v-for="daySchedule in schedule.daysSchedule"
      :key="daySchedule.dayTitle"
      v-bind.sync="daySchedule"
    ></day-schedule>
        </v-row>
  </v-row>
</v-container>
</template>

<script>
import DaySchedule from '~/components/elements/day-schedule.vue'

export default {
  data () {
    return {
      workTimeTitle: 'Work time',
      breakTimeTitle: 'Break time',
      periods: [
        {dn: 'days', value: 0},
        {dn: 'weeks', value: 1}
      ],
      selectedPeriod: 0,
      every: '1'
    }
  },

  computed: {
    showDaysSch: function () { return this.selectedPeriod === 1 }
  },

  components: {
    DaySchedule
  },

  props: {
    schedule: {}
  }
}
</script>>