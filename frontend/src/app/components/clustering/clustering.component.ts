import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable, delay} from 'rxjs';
import { LoadingService } from 'src/app/loading.service';
import Chart, { ChartDataset, ChartTypeRegistry } from 'chart.js/auto';

interface Clusters {
  value: string;
  viewValue: string;
}

interface Users {
  firstName: string;
  lastName: string;
  activity: string;
  university: string;
  facultyName: string;
  vkIdString: string;
}

interface Point{
  pointX: number;
  pointY: number;
  color: string;
}

@Component({
  selector: 'app-clustering',
  templateUrl: './clustering.component.html',
  styleUrls: ['./clustering.component.css']
})

export class ClusteringComponent {

  selectedValue: string | undefined;
  selectedClust: string | undefined;
  public users: Users[] = [];
  public pointsInterests: Point[] = [];
  public pointsFaculties: Point[] = [];
  public pointsUniversities: Point[] = [];
  public points: Point[] = [];


  constructor(private http: HttpClient) {}

  getDataFromClustering(link: string) : Observable<Users[]> {
    return this.http.get<Users[]>(link);
  }

  getPointsFromClustering(link: string) : Observable<Point[]> {
    return this.http.get<Point[]>(link);
  }

  clustering() : void {
    var btn = document.querySelector('[name="buttonSearch"]');
    this.getPointsInterests();
    this.getPointsUniversities();
    this.getPointsFaculties();

    if (this.selectedValue == 'activity-0') {
      this.getDataFromClustering("https://" + location.hostname + ":7104/api/Clussterization/InterestsClusters")
      .subscribe(users => this.users = users);
      show('list');
      show('btn-search');
      hide('header_ref');
    } else if (this.selectedValue == 'faculties-1') {
      this.getDataFromClustering("https://" + location.hostname + ":7104/api/Clussterization/UniversitiesClusters")
      .subscribe(users => this.users = users);
      show('list');
      hide('btn-search');
      hide('MyChart');
      hide('header_ref');
    } else if (this.selectedValue == 'univercity-2') {
      this.getDataFromClustering("https://" + location.hostname + ":7104/api/Clussterization/FacultiesClussters")
      .subscribe(users => this.users = users);
      show('list');
      hide('btn-search');
      hide('MyChart');
      hide('header_ref');
    } else {
      alert('Выберите фильтр');
    }
  }

  getPointsInterests(): void{
    this.getPointsFromClustering("https://" + location.hostname + ":7104/api/Clussterization/GetPointsInterests")
      .subscribe(points => this.pointsInterests = points);
  }

  getPointsUniversities(): void{
    this.getPointsFromClustering("https://" + location.hostname + ":7104/api/Clussterization/GetPointsUniversities")
      .subscribe(points => this.pointsUniversities = points);
  }

  getPointsFaculties(): void{
    this.getPointsFromClustering("https://" + location.hostname + ":7104/api/Clussterization/GetPointsFaculties")
      .subscribe(points => this.pointsFaculties = points);
  }

  stop() {
    var btn = document.querySelector('[name="buttonSearch"]');
    
    if (btn != null && this.users.length != 0) {
      btn.removeAttribute('disabled');
    }
  }

  cluster: Clusters[] = [
    {value: 'activity-0', viewValue: 'По интересам'},
    {value: 'faculties-1', viewValue: 'По факультетам'},
    {value: 'univercity-2', viewValue: 'По университатам'},
  ];

  public chart: any;

drawGraph(){
  if (this.selectedValue == 'activity-0') {
    this.points=this.pointsInterests;
    
  } else if (this.selectedValue == 'faculties-1') {
    this.points=this.pointsUniversities;
    
  } else if (this.selectedValue == 'univercity-2') {
    this.points=this.pointsFaculties;
  }
  this.createChart();
  show('MyChart');
}

  createChart(){
    var dataset = new Array();
    var data = new Array();
    var color = this.points[0].color;
    this.points.forEach( (value) => {
      if(color!=value.color){
        dataset.push({data, color});
        data = new Array();
      }
      console.log(color, value.color);
      var x = value.pointX + this.randomInt(-3, +3);
      var y = value.pointY + this.randomInt(-3, +3);
      data.push({x, y});
      color = value.color;
    });
    console.log(dataset);
    this.chart = new Chart("MyChart", {
      type: 'scatter',
      data: {
        datasets: dataset
      },
      options: {
        plugins: {
          legend: {
            display: false,
            position: "right"
          }
        }
      }
    });
  }

  randomInt(min: number, max: number) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }
}