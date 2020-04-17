## Criando Web API
1. Instalar dotnet core 3.0

2. Criar WebAPI
dotnet new webapi -o TodosAPI

3. Entrar na pasta
cd TodosAPI

4. Adicionar pacote Microsoft.EntityFrameworkCore.InMemory
dotnet add package Microsoft.EntityFrameworkCore.InMemory

5. Criar classe Models.Todo
```csharp
using System;

namespace TodosAPI.Models
{
    public class Todo
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string CreationDate { get; set; }

        public bool Done { get; set; }

        public string Priority { get; set; }
    }
}
```

6. Criar Data.ApiContext:
```csharp
using Microsoft.EntityFrameworkCore;

namespace TodosAPI.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
          : base(options)
        {}
        public DbSet<Todo> Todos { get; set; }
    }
}
```

7. Incluir novo serviço em Startup.ConfigureServices, antes de services.AddControllers()
```csharp
services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase());
```

8. Alterar também o Startup.cs para incluir CORS:
```csharp
app.UseCors(c => c.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
```

9. Adicionar code generator
dotnet tool install --global dotnet-aspnet-codegenerator --version 3.0.0
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 3.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 3.0.3
dotnet add package Microsoft.EntityFrameworkCore.Design --version 3.0.3

10. Adicionar Controller:
dotnet-aspnet-codegenerator -p TodosAPI.csproj controller -name TodoController -api -m TodosAPI.Models.Todo -dc TodosAPI.Data.ApiContext -outDir Controllers -namespace TodosAPI.Controllers

11. Iniciar a aplicação
dotnet run

12. Usar Postman para testar

## Criando Front-End
1. Instalar @angular/cli. Escolher a opção Angular routing e SCSS para estilos.
npm install -g @angular/cli

2. Após a criação do projeto, acessar a pasta "TodosWeb"
cd TodosWeb

3. Adicionar módulo material. Escolher opções padrão na instalação.
ng add @angular/material

4. Adicionar pacote NPM ngx-loading:
npm add package ngx-loading

5. Alterar arquivo src/app/app.module.ts:
```javascript
...
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatIconModule} from '@angular/material/icon';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatSnackBarModule} from '@angular/material/snack-bar';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxLoadingModule, ngxLoadingAnimationTypes } from 'ngx-loading';
import { TodoListComponent } from './todo-list/todo-list.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,

    // CDK
    A11yModule,
    BidiModule,
    ObserversModule,
    OverlayModule,
    PlatformModule,
    PortalModule,
    ScrollingModule,
    CdkStepperModule,
    CdkTableModule,

    // Material
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatCheckboxModule,
    MatSnackBarModule,

    NgxLoadingModule.forRoot({
      animationType: ngxLoadingAnimationTypes.circle,
      backdropBackgroundColour: 'rgba(0,0,0,0.6)',
      backdropBorderRadius: '4px',
      primaryColour: '#ffffff',
      secondaryColour: '#ffffff',
      tertiaryColour: '#ffffff'
    })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

6. Adicionar componente TodoList
ng g component TodoList

7. Adicionar componente em declarations do AppModule:
```javascript
@NgModule({
  declarations: [
    AppComponent,
    TodoListComponent
  ],
  ...
```
  
8. Incluir uma rota para acessar o componente TodoList. Alterar o arquivo app\app-routing.module.ts:
```javascript
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { TodoListComponent } from './todo-list/todo-list.component';

const routes: Routes = [
  { path:'', component:AppComponent},
  { path:'index.html', component:AppComponent},
  { path:'index', component:AppComponent},
  { path: 'todos', component: TodoListComponent }  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
```

9. Alterar o arquivo app\app.component.html para remover conteúdo padrão do Angular:
```html
<router-outlet></router-outlet>
```

10. Testar a aplicação http://localhost:4200/todos:
ng serve

11. Adicionar app\shared\models\todo.model.ts:
```javascript
export class Todo {
    id: number;
    todoTitle: string;
    creationDate: string;
    parsedDate: Date;
    done: boolean;
    priority: number;       
}
```

12. Adicionar o serviço Todo para acesso a API:
ng g service Todo

13. Alterar o todo.service.ts:
```javascript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders  } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { Todo } from './shared/models/todo.model';

var httpOptions = {headers: new HttpHeaders({"Content-Type": "application/json"})};

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private url = 'https://localhost:5001/api/Todo';

  constructor(
    private http: HttpClient
  ) { }

  public create(newTodo: any): Observable<any> {
    return this.http.post(this.url, newTodo, httpOptions);
  }

  public retrievetById(id: number): Observable<Todo> {
    return this.http.get<Todo>(`${this.url}/${id}`);
  }

  public retrieveAll(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.url);
  }

  public update(updatedTodo: Todo): Observable<Todo> {
    return this.http.put<Todo>(`${this.url}/${updatedTodo.id}`, updatedTodo, httpOptions);
  }

  public delete(id: number): Observable<number> {
    return this.http.delete<number>(`${this.url}/${id}`);
  }
}
```

14. Adicionar modulo HttpClient em app.module.ts
```javascript
import { HttpClientModule } from '@angular/common/http';
```

15. Adicionar também em @NgModule imports:
```javascript
HttpClientModule
```
    
16. Alterar todo-list.component.html
```html
<div>
  <p *ngIf="!todoListArray?.length">
    Nenhuma tarefa definida.
  </p>
  <mat-card class="todoItem">
    <mat-card-actions layout="row" align="center">
        <button mat-button>
            <a [routerLink]="[ '/todo/']">
                <mat-icon>add</mat-icon> CREATE TODO
            </a>
        </button>
    </mat-card-actions>
  </mat-card>
  <ng-container *ngFor="let item of todoListArray; let itemIndex = index">
      <mat-card class="todoItem">
          <mat-card-header>
              <div mat-card-avatar>
              <span class="md-headline {{item.priority==1 ? 'high' : item.priority==2 ? 'medium' : 'low'}}">
                  <mat-icon>{{ item.priority==1 ? 'error' : item.priority==2 ? 'warning' : 'info'}}</mat-icon>
              </span>
              </div>
              <mat-card-title>
              {{item.title}}
              </mat-card-title>
              <mat-card-subtitle>Criado em {{item.creationDate}}</mat-card-subtitle>
          </mat-card-header>
          <mat-card-actions layout="row" align="end">              
              <button mat-button (click)="concludeTodo(itemIndex)" >
                  <mat-icon>{{ item.done ? 'delete' : 'check'}}</mat-icon>
              </button>
          </mat-card-actions>
      </mat-card>
  </ng-container>
</div>

<ngx-loading [show]="loading"></ngx-loading>
```

17. Agora vamos alterar o todo.list.component.ts
```javascript
import { Component, OnInit } from '@angular/core';
import { TodoService } from 'src/app/todo.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Todo } from '../shared/models/todo.model';

@Component({
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.scss']
})
export class TodoListComponent implements OnInit {

  public loading: boolean = false;
  public todoListArray: Todo[]
  constructor(
    private todoService: TodoService,
    private _snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    this.getTodos();
  }

  public getTodos(): void {
    this.loading = true;
    setTimeout(() => {
      this.todoService.retrieveAll()
      .subscribe(items => {
        this.loading = false;
        this.todoListArray = items;
      },(error) => {
        this.loading = false;
        console.log(error);
      });
    },2000);
  }

  public concludeTodo(todoIndex: number): void {
    this.loading = true;
    setTimeout(() => {
      let todo = this.todoListArray[todoIndex];
      if (todo.done) {
        this.todoService.delete(todo.id)
        .subscribe(() => {
          this.successMessage("Todo removido com sucesso");
          this.getTodos();
        }
        ,(error) => this.errorMessage(error, "Erro ao atualizar Todo"));
      }
      else {
        todo.done = true;
        this.todoService.update(todo)
        .subscribe((data: Todo) => this.successMessage("Todo atualizado com sucesso")
        ,(error) => this.errorMessage(error, "Erro ao atualizar Todo"));
      }
    },2000);
  }

  private successMessage(successMessage: string)
  {
    this.loading = false;
    console.log(successMessage);
    this._snackBar.open(successMessage, null, {
      duration: 2000,
    });
  }

  private errorMessage(error: string, errorMessage: string)
  {
    this.loading = false;
    console.log(error);
    this._snackBar.open(errorMessage, null, {
      duration: 2000,
    })
  }

}
```

> Note que estamos usando o MatSnackBar para exibir as mensagens de erro e sucesso. É possível aprimorar melhor esse procedimento, mas para o propósito deste tutorial, nos iremos ater a esse nível de feedback.
> Note agora como está implementado o concludeTodo() de forma condicional quanto a atualização para done ou remoção do todo.

18. Agora vamos alterar o arquivo todo-list.component.scss:
```css
todoItem {
    margin-bottom: 20px;
    width: 400px;
}

mat-card-header  {
    size: 2em;

    mat-icon {
        font-size: 3em;
    }

    .high {
        color:red;
    }

    .medium {
        color: yellow;
    }

    .low {
        color: lightgreen;
    }
}
```

19. Vamos testar agora:
ng serve

20. Adicionar componente todo editor:
ng g component TodoEditor

21. Incluir rotas para app.routing.module.ts
```javascript
{ path: 'todo/:todoId', component: TodoEditorComponent },
{ path: 'todo', component: TodoEditorComponent }
```

22. Modificar o card do todo em todo-list.component.html:
```html
<mat-card class="todoItem">
    <mat-card-actions layout="row" align="center">
        <button mat-button>
            <a [routerLink]="[ '/todo/']">
                <mat-icon>add</mat-icon> CREATE TODO
            </a>
        </button>
    </mat-card-actions>
</mat-card>
```

23. Incluit também o todo-list.component.html para chamar o todo-editor.component.html
```html
<button mat-button>
	<a [routerLink]="[ '/todo', item.id]">
		<mat-icon>edit</mat-icon>
	</a>
</button>
```

24. Alterar todo-editor.component.html para incluir formulário de criação do Todo:
```html
<mat-card>
  <mat-card-title>
    Tasks
  </mat-card-title>
  <mat-card-content>
    <form [formGroup]="todoForm">
      <input type="hidden" formControlName="id"/>
      <input type="hidden" formControlName="creationDate"/>
      <mat-form-field>
        <input matInput placeholder="Titulo" formControlName="title">
      </mat-form-field>
      <br/>
      <mat-form-field>
        <input matInput [matDatepicker]="picker" placeholder="Data de criação" formControlName="parsedDate">
        <mat-datepicker-toggle matSuffix [for]="picker" ></mat-datepicker-toggle>
        <mat-datepicker #picker></mat-datepicker>
      </mat-form-field>
      <br/>
      <mat-checkbox formControlName="done">Finalizado</mat-checkbox>
      <br/>
      <br/>
      <mat-form-field>
        <mat-label>Prioridade</mat-label>
        <mat-select formControlName="priority">
          <mat-option value="1">
            Alta
          </mat-option>
          <mat-option value="2">
            Média
          </mat-option>
          <mat-option value="3">
            Baixa
          </mat-option>
        </mat-select>
      </mat-form-field>
    </form>
  </mat-card-content>
  <mat-card-actions class="center-text">
      <button mat-raised-button color="primary" class="full-width" (click)="doRequest()" [disabled]="!todoForm.valid">
        SALVAR
      </button>
  </mat-card-actions>
</mat-card>

<ngx-loading [show]="loading"></ngx-loading>
```

25. Note que incluimos a chamada para o método "doRequest()". Esse método deve ser especificado no Javascript referenciado por este HTML. No caso, quando é algo específico para o componente, podemos especificar o "todo-editor.component.ts":
```javascript
import { Component, Input, OnInit } from '@angular/core';
import { TodoService } from 'src/app/todo.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Todo } from '../shared/models/todo.model';
import { ActivatedRoute } from '@angular/router'

@Component({
  selector: 'app-todo-editor',
  templateUrl: './todo-editor.component.html',
  styleUrls: ['./todo-editor.component.scss']
})
export class TodoEditorComponent implements OnInit {

  private todoId: string;
  public todoForm: FormGroup;
  public loading: boolean = false;
  constructor(
    private todoService: TodoService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private _snackBar: MatSnackBar
  ) {
    this.todoForm = this.formBuilder.group(
      {
        id: [''],
        title: ['', Validators.required],
        creationDate: [''],
        parsedDate: [new Date(), Validators.required],
        done: [false, Validators.required],
        priority: ["1", Validators.required]
      }
    );
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.todoId = params['todoId'];
      if (this.todoId){
        this.getTodo(this.todoId);
      }
    });
  }

  public getTodo(id: any): void {
    this.loading = true;
    setTimeout(() => {
      this.todoService.retrievetById(id)
      .subscribe((data: Todo) => {
        this.loading = false;
        data.parsedDate = new Date(data.creationDate);
        this.todoForm.setValue(data);
      },(error) => {
        this.loading = false;
        console.log(error);
      });
    },2000);
  }

  public showLoading(): void {
    this.loading = true;
    setTimeout(() => {
      this.loading = false;
    }, 1000);
  }

  public doRequest(): void {
    this.loading = true;
    setTimeout(() => {
      let data: Todo = this.todoForm.value;
      data.creationDate = data.parsedDate.toDateString();
      if (data.id === undefined || data.id == 0)
      {
        data.id = 0;
        this.todoService.create(data)
        .subscribe((data: Todo) => this.successMessage("Todo criado com sucesso")
        ,(error) => this.errorMessage(error, "Erro ao inserir novo Todo"));
      }
      else
      {
        this.todoService.update(data)
        .subscribe((data: Todo) => this.successMessage("Todo atualizado com sucesso")
        ,(error) => this.errorMessage(error, "Erro ao atualizar Todo"));
      }
    },2000);
  }

  private successMessage(successMessage: string)
  {
    this.loading = false;
    console.log(successMessage);
    this._snackBar.open(successMessage, null, {
      duration: 2000,
    });
  }

  private errorMessage(error: string, errorMessage: string)
  {
    this.loading = false;
    console.log(error);
    this._snackBar.open(errorMessage, null, {
      duration: 2000,
    });
  }
}
```

26. Agora vamos testar as alterações.
ng serve