<template>
<v-container container--fluid>
    <v-row wrap>
        <v-col sm3>
            <v-text-field type="number" name="repeat-input" label="Repeat every"
            id="repeatNumber" min=0 max=500 step=1 maxlength=3 v-model="settings.every"
          ></v-text-field>
        </v-col>
        <v-col sm3>
            <v-select :items="periods" item-value="value" item-text="dn" v-model="selectedPeriod" @change="updatePeriod" label="Select" single-line></v-select>
        </v-col>
        <v-col sm6>
        </v-col>
        <v-col sm6>
            <week-days :weekDays="settings.repeatOn"></week-days>
        </v-col>
        <v-col sm6>
        </v-col>
        <v-col sm8>
         <v-radio-group v-model="settings.endcondition">

             <v-radio :label="'Never'" :key="'never'" :value="'never'"></v-radio> 
              <v-row wrap>
              <v-col sm2><v-radio :label="'On'" :key="'on'" :value="'on'"></v-radio></v-col> <v-col sm4><date-picker  dateFormat="MMMM, DD YYYY" title="Date"></date-picker></v-col>
              </v-row>
              <v-row wrap>
             <v-col sm2><v-radio :label="'After'" :key="'after'" :value="'after'"></v-radio></v-col> 
             <v-col sm1>
                  <v-text-field type="number" name="repeat-input" label=""
            id="repeatOccurrences" min=0 max=1000 step=1 maxlength=4 v-model="settings.endoccurrencesCount" ></v-text-field>
             </v-col>
              </v-row>
     
         </v-radio-group>
        </v-col>
    </v-row>
</v-container>

    
</template>

<script>
import WeekDays from '~/components/elements/weekdays.vue'
import DatePicker from '~/components/elements/datepicker.vue'
export default {
  data () {
    return {
      periods: [
        {dn: 'days', value: 0},
        {dn: 'weeks', value: 1},
        {dn: 'months', value: 2},
        {dn: 'years', value: 3}
      ],
      selectedPeriod: this.settings.period,
      showWeeks: false,
      radioGroup: 'on'
    }
  },
  props: {
    settings: {}
  },
  components: {
    WeekDays,
    DatePicker
  },
  methods: {
    getActiveStatus (index) {
      return this.weekdays[index].active
    },

    chooseDay (index) {
      var current = this.weekdays[index].active
      this.weekdays[index].active = !current
    },

    updatePeriod (period) {
      this.selectedPeriod = period
      if (period === 1) this.showWeeks = true
    }

  }
}
</script>



