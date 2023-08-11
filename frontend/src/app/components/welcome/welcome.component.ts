import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable} from 'rxjs';
import { FormBuilder, FormGroup } from '@angular/forms';

interface Users {
  firstname?: string;
  lastname?: string;
  activities?: string;
  link?: string;
}

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css']
})
export class WelcomeComponent {

  public users?: Users[] = [];
  public userForm: FormGroup; 
  link: string = "";

  constructor(private http: HttpClient, private fb: FormBuilder) {
    this.userForm = this.fb.group({
      link: ''
    });
  }

  setUsers() : void {    
    const link: string = this.userForm.get('link')?.value;
    const id: string = link.substring(link.lastIndexOf('/') + 1);
    const linkVk = "https://" + location.hostname + ":7104/api/Clussterization/SetUser/" + id;
    this.http.get("https://" + location.hostname + ":7104/api/Clussterization/SetUser/" + linkVk);
  }

  getDataFromClustering(id: string) : Observable<Users[]> {
    const linkVk = "https://" + location.hostname + ":7104/api/Clussterization/SetUser/" + id;
    return this.http.get<Users[]>(linkVk);
  }

  setUser() : void {
    const linkVk: string = this.userForm.get('link')?.value;
    const id: string = linkVk.substring(linkVk.lastIndexOf('/') + 1);
    this.getDataFromClustering(id)
    .subscribe(users => this.users = users);
  }
}