import React from 'react';
import { Chart } from 'primereact/chart'; 

function useStatisticsData(items) {
    const data = React.useMemo (() => ({
        labels: items.map(i => i.Name),
        datasets: [
            {
                label: 'Applications',
                backgroundColor: '#42A5F5',
                data: items.map(i => i.MinutesRan / 60)
            }
        ]
    }), [items]);

    data.datasets[0].data.push(Math.max(data.datasets[0].data) + 0.5);
    return data;
}

export function UsageStatistics(props) {
    const [statistics, setStatistics] = React.useState([]);
    const chartData = useStatisticsData(statistics);

    React.useEffect(() => {
        async function getData() {
            const result = await fetch('https://localhost:42426/trackerapi');
            setStatistics(await result.json());
        }
        getData();
        const handle = setInterval(() => {
            getData();
        }, 60000);

        return () => clearInterval(handle);

    }, []);
    return(
        <Chart type={props.chartType} data={chartData} options={{height: 100, maintainAspectRatio: false, aspectRatio: 1, plugins: {legend: {position: 'bottom'}}}} />
    )    
}

