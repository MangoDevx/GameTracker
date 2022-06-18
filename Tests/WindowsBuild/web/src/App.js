import React from 'react';

import './App.css';
import "primereact/resources/themes/lara-light-indigo/theme.css";  //theme
import "primereact/resources/primereact.min.css";                  //core css
import "primeicons/primeicons.css";                                //icons
import "/node_modules/primeflex/primeflex.css";

import { Menubar } from 'primereact/menubar';

import 'primereact/resources/themes/bootstrap4-light-blue/theme.css';
import { UsageStatistics } from './UsageStatistics';

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
        <div className="grid justify-content-center m-0">
          <div className="col-10">
            <UsageStatistics chartType="bar"/>
          </div>
          <div className="col-4">
            <UsageStatistics chartType="pie"/>
          </div>
          <div className="col-4">
            <UsageStatistics chartType="polarArea"/>
          </div>
          <div className="col-4">
            <UsageStatistics chartType="doughnut"/>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
