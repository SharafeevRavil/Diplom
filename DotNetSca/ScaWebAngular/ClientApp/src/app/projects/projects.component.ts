import {Component, Inject, OnInit} from '@angular/core';
import {Project} from "../../models/project";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit {
  public projects: Project[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Project[]>(baseUrl + 'api/projects').subscribe(result => {
      this.projects = result;
      this.projects = [{
        name: "aa",
        description: "aaaa",
        id: 124
      }];
    }, error => console.error(error));
  }

  ngOnInit(): void {
  }

}
