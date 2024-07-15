// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include <iostream>

#pragma comment(lib, "Ws2_32.lib")
#pragma warning(disable : 4996)

#define BBDECL extern "C" _declspec(dllexport)
#define BBCALL _stdcall

SOCKET ConnectSocket = INVALID_SOCKET;
struct sockaddr_in serverAddr;

BBDECL int BBCALL ConnectToServer(const char* ipAddress, int port) {
    WSADATA wsaData;
    int iResult;

    iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0) {
        return 1; // WSAStartup failed
    }

    ConnectSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (ConnectSocket == INVALID_SOCKET) {
        WSACleanup();
        return 2; // Error at socket()
    }

    ZeroMemory(&serverAddr, sizeof(serverAddr));
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(port);
    inet_pton(AF_INET, ipAddress, &serverAddr.sin_addr);

    return 0; // Successfully connected
}

BBDECL int BBCALL SendDataToServer(const char* data) {
    int iResult = sendto(ConnectSocket, data, (int)strlen(data), 0, (struct sockaddr*)&serverAddr, sizeof(serverAddr));
    if (iResult == SOCKET_ERROR) {
        closesocket(ConnectSocket);
        WSACleanup();
        return 1; // sendto failed
    }
    return 0; // Data sent successfully
}

BBDECL int BBCALL IsNewDataAvailable() {
    fd_set readfds;
    FD_ZERO(&readfds);
    FD_SET(ConnectSocket, &readfds);

    struct timeval timeout;
    timeout.tv_sec = 0;  // No wait time
    timeout.tv_usec = 0;

    int iResult = select(0, &readfds, NULL, NULL, &timeout);
    if (iResult > 0 && FD_ISSET(ConnectSocket, &readfds)) {
        return 1; // New data is available
    }
    return 0; // No new data
}

BBDECL const char* BBCALL ReceiveDataFromServer() {
    static char recvbuf[512];
    struct sockaddr_in fromAddr;
    int fromLen = sizeof(fromAddr);
    int iResult = recvfrom(ConnectSocket, recvbuf, 512, 0, (struct sockaddr*)&fromAddr, &fromLen);
    if (iResult > 0) {
        recvbuf[iResult] = '\0'; // Null-terminate the string
        return recvbuf;
    }
    else if (iResult == 0) {
        return "Connection closed";
    }
    else {
        return "recvfrom failed";
    }
}

BBDECL void BBCALL DisconnectFromServer() {
    closesocket(ConnectSocket);
    WSACleanup();
}