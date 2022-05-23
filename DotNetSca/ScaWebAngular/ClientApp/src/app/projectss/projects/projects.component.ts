import {Component, Inject, OnInit} from '@angular/core';
import {Project} from "../../../models/project";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit {
  public projects: Project[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string,
              private router: Router) {
    http.get<Project[]>(baseUrl + 'api/projects').subscribe(result => {
      this.projects = result;
    }, error => console.error(error));
  }

  ngOnInit(): void {
  }

  open(id: number) {
    this.router.navigateByUrl('/projects/' + id)
  }
}
