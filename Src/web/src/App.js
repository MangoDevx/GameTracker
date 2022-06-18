import React from 'react';
import ReactDOM from 'react-dom/client';

import './App.css';
import "primereact/resources/themes/lara-light-indigo/theme.css";  //theme
import "primereact/resources/primereact.min.css";                  //core css
import "primeicons/primeicons.css";                                //icons
import "/node_modules/primeflex/primeflex.css";

import { Menubar } from 'primereact/menubar';
import { Chart } from 'primereact/chart'; 

import 'primereact/resources/themes/bootstrap4-light-blue/theme.css';

function App() {
  const menubarItems = [
    {
      label: 'Graphs',
      icon: 'pi pi-chart-bar'
    }
  ]

  return (
    <div className="App">
      <Menubar model={menubarItems}/>
      <div className='App-header'>
        <div className="grid Chart-display">
        <BarChart />
        </div>
      </div>
    </div>
  );
}

export class BarChart extends React.Component {
  constructor(props) {
    super(props);

    this.testData = {
      labels: ["One", "Two", "Three", "Four", "Five"],
      datasets: [
        {
          label: "Hours Played",
          backgroundColor: '#42A5F5',
          data: [30, 10, 20, 50, 40]
        }
      ]
    };

    const maxInput = Math.max(...this.testData.datasets[0].data) + 10;
    this.testData.datasets[0].data.push(maxInput);
    
    this.options = this.getChartOptions();
  }

  getChartOptions() {
    let options = {
      maintainAspectRatio: false,
      aspectRatio: 1,
    }
    return options;
  }

  render() {
    return (
      <div className='chart'>
        <h4>Bar Chart</h4>
        <Chart type='bar' data={this.testData} options={this.options} />
      </div>
    )
  }
}

export default App;
