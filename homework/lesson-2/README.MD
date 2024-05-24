## Your homework for Lesson 2:

[Complete the Create Plugins lesson on learn](https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/) 

[Read the *Understanding AI Plugins in Semantic Kernel* document](https://learn.microsoft.com/en-us/semantic-kernel/agents/plugins/?tabs=Csharp)
  
   **Important Note about Planners**
   <details>
      <summary><u>Details.</u> (<i>click to expand</i>)</summary>
      <!-- have to be followed by an empty line! -->
 
     The StepWise and HandleBar planners are being deprecated.  We are recommending that customer leverage Auto Invoke as opposed to user either of these planners.  The SK team has been working on a new planner that will replace these and it has been proven to be much more efficient.  In most cases we are finding that a planner is simply not needed and using multiple plugins with Auto Invoke often proves to be much more efficient.  If you need more control you can use an *intent* pattern to control flow.
          
  </details>

[Prompt Template Language](https://learn.microsoft.com/en-us/semantic-kernel/prompts/prompt-template-syntax)

The Semantic Kernel prompt template language is a simple way to define and compose AI functions using plain text. You can use it to create natural language prompts, generate responses, extract information, invoke other prompts or perform any other task that can be expressed with text.

[Creating Functions from Prompts and adding variables](https://learn.microsoft.com/en-us/semantic-kernel/prompts/templatizing-prompts?tabs=Csharp)

The Semantic Kernel prompt template language is very powerful, you can create tokens that will automatically be replace with input parameters.  

### Knowledge Check
When using prompts in Semantic Kernel, is it required to use the SK Prompt when calling InvokeAsync()?
  <details>
      <summary><u>Answer</u> (<i>click to expand</i>)</summary>
      <!-- have to be followed by an empty line! -->
   
       No. As demonstrated in the code examples, you can use a variety of techniques for prompts i.e. interpolated strings, just a regular string as a prompt, load from file or inline.
            
  </details>

If you want your AI solution to invoke custom logic, are you required to leverage a plugin?
  <details>
      <summary><u>Answer</u> (<i>click to expand</i>)</summary>
      <!-- have to be followed by an empty line! -->
        Yes.  You need to use native functions for this and this does require that you build a plugin for this.  Take a look at the Plugin Examples in the Lesson-2 solution for examples. 
       
  </details>

[🔼 Home ](/README.md) | [Next Homework 3 ▶](/homework/lesson-3/README.md)
