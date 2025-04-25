#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <signal.h>
#include <fcntl.h>
#include <sys/types.h>
#include <sys/wait.h>

#define LINE_SIZE 1024

// Handler pentru semnalul primit de părinte
void handle_sig(int sig, siginfo_t *info, void *ucontext) {
    printf("Parinte: Am primit %d aparitii ale caracterului 'a' de la copil.\n", info->si_value.sival_int);
}

int main() {
    int pipefd[2];
    pid_t pid;

    if (pipe(pipefd) == -1) {
        perror("pipe");
        exit(EXIT_FAILURE);
    }

    // Setare handler pentru semnalul SIGRTMIN
    struct sigaction sa;
    sa.sa_sigaction = handle_sig;
    memset(s,0,sizeof(s));
    if (sigaction(SIGRTMIN, &sa, NULL) == -1) {
        perror("sigaction");
        exit(EXIT_FAILURE);
    }

    pid = fork();
    if (pid < 0) {
        perror("fork");
        exit(EXIT_FAILURE);
    }

    if (pid == 0) {
        // COPIL
        close(pipefd[1]);  // Închidem capătul de scriere

        char buffer[LINE_SIZE];
        int total_a = 0;

        while (1) {
            ssize_t len = read(pipefd[0], buffer, LINE_SIZE);
            if (len <= 0) break;

            for (ssize_t i = 0; i < len; ++i) {
                if (buffer[i] == 'a') {
                    total_a++;
                }
            }
        }

        close(pipefd[0]);

        // Trimitem semnalul către părinte
        
        kill(getppid(), SIGRTMIN, total_a); // Folosim kill cu sigqueue
        exit(0);
    } else {
        // PARINTE
        close(pipefd[0]);  // Închidem capătul de citire

        FILE *file = fopen("input.txt", "r");
        if (!file) {
            perror("fopen");
            exit(EXIT_FAILURE);
        }

        char line[LINE_SIZE];
        while (fgets(line, LINE_SIZE, file)) {
            write(pipefd[1], line, strlen(line));
        }

        fclose(file);
        close(pipefd[1]);

        // Așteptăm copilul să termine și semnalul să vină
        wait(NULL);
    }

    return 0;
}
